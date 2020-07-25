
class_name Solver

var Lib = preload("Vecmath.gd")
var Jt = preload("Joint.gd")
var endpoint: Spatial
var joints: Array

func _init(ep, jts):
	endpoint = ep
	joints = jts

func compute_jacobian(eps: float=1e-3) -> Array:
	var result = Lib.make_matrix(3, joints.size())
	var current = endpoint.global_transform.origin
	var index: int = 0
	for jj in joints:
		var j: Jt = jj
		j.save_state()
		j.update_parameter(eps)
		var new_pos = endpoint.global_transform.origin
		var diff = (new_pos - current) / eps
		j.reset_state()
		result[0][index] = diff.x
		result[1][index] = diff.y
		result[2][index] = diff.z
		index += 1
	return result

func random_unit_vector(dims: int) -> Array:
	var result = Array()
	result.resize(dims)
	for i in range(dims):
		result[i] = 2 * randf() - 1
	return Lib.normalized(result)

func svd_1d(matrix: Array, eps: float=1e-5) -> Array:
	var _m: int = matrix.size()
	var n: int = matrix[0].size()
	var last_iteration: Array
	var curr_iteration: Array = random_unit_vector(n)
	var iterations: int = 0
	var b = Lib.mul_mat(Lib.transpose(matrix), matrix)
	while iterations < 100:
		last_iteration = curr_iteration
		curr_iteration = Lib.mul_vec(b, last_iteration)
		curr_iteration = Lib.normalized(curr_iteration)
		if abs(Lib.dot(last_iteration, curr_iteration)) > 1 - eps:
			break
		iterations += 1
	return curr_iteration

func svd(matrix: Array, eps: float=1e-5):
	var m: int = matrix.size()
	var n: int = matrix[0].size()
	var sigmas = Array()
	sigmas.resize(min(m, n))
	var us = Lib.make_matrix(m, m)
	var vs = Lib.make_matrix(n, n)
	var remaining = Lib.copy(matrix)
	for i in range(min(m, n)):
		var v = svd_1d(remaining, eps)
		var u = Lib.mul_vec(matrix, v)
		var contrib = Lib.outer_product(u, v)
		var s = Lib.length(u)
		u = Lib.normalized(u)
		for j in range(m):
			us[j][i] = u[j]
		for j in range(n):
			vs[j][i] = v[j]
		sigmas[i] = s
		remaining = Lib.scale_add(remaining, contrib, -1)
	return [sigmas, us, vs]

func compute_pseudoinverse(matrix: Array, eps: float=1e-5):
	var m = matrix.size()
	var n: int = matrix[0].size()
	var decomp = svd(matrix)
	var s: Array = decomp[0]
	var u = decomp[1]
	var v = decomp[2]
	for i in range(s.size()):
		if s[i] > eps:
			s[i] = 1 / s[i]
	var ss = Lib.make_matrix(n, m)
	for i in range(min(m, n)):
		ss[i][i] = s[i]
	return Lib.mul_mat(
		Lib.mul_mat(v, ss),
		Lib.transpose(u))

func compute_ik(target: Vector3, damp: float):
	var jacobian = compute_jacobian()
	var curr = endpoint.global_transform.origin
	var dpos = target - curr
	dpos = [dpos.x, dpos.y, dpos.z]
	var damped = Lib.scale_add(
		Lib.mul_mat(Lib.transpose(jacobian), jacobian),
		Lib.identity(jacobian[0].size()), damp * damp
	)
	var inv_damped = compute_pseudoinverse(damped)
	var dconfig = Lib.mul_vec(
		inv_damped,
		Lib.mul_vec(Lib.transpose(jacobian), dpos)
	)
	return dconfig

func test_pseudoinverse(mat):
	var inv = compute_pseudoinverse(mat)
	var ax = Lib.mul_mat(mat, inv)
	var xa = Lib.mul_mat(inv, mat)
	var axa = Lib.mul_mat(ax, mat)
	var xax = Lib.mul_mat(xa, inv)
	assert(Lib.is_zero(Lib.scale_add(axa, mat, -1)))
	assert(Lib.is_zero(Lib.scale_add(xax, inv, -1)))
	assert(Lib.is_zero(Lib.scale_add(ax, Lib.transpose(ax), -1)))
	assert(Lib.is_zero(Lib.scale_add(xa, Lib.transpose(xa), -1)))

func test_svd(mat):
	var decomp = svd(mat)
	var s = Lib.diag(decomp[0])
	var u = decomp[1]
	var v = decomp[2]
	var res = Lib.mul_mat(Lib.mul_mat(u, s), Lib.transpose(v))
	assert(Lib.is_zero(Lib.scale_add(mat, res, -1)))
