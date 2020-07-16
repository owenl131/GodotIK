
class_name Lib

static func dot(v1: Array, v2: Array) -> float:
	assert(v1.size() == v2.size())
	var result: float = 0
	for i in range(v1.size()):
		result += v1[i] * v2[i]
	return result

static func len_sq(v: Array) -> float:
	return dot(v, v)

static func length(v: Array) -> float:
	return sqrt(len_sq(v))

static func extract_row(m: Array, row: int) -> Array:
	return m[row]

static func extract_col(m: Array, col: int) -> Array:
	return extract_row(transpose(m), col)

static func normalized(v: Array, eps: float=1e-6) -> Array:
	var norm = length(v)
	if norm < eps:
		return v
	for i in range(v.size()):
		v[i] = v[i] / norm
	return v

static func make_matrix(rows: int, cols: int) -> Array:
	var result = Array()
	result.resize(rows)
	for i in range(rows):
		result[i] = Array()
		result[i].resize(cols)
		for j in range(cols):
			result[i][j] = 0
	return result

static func copy(m: Array) -> Array:
	var result = make_matrix(m.size(), m[0].size())
	for i in range(m.size()):
		for j in range(m[0].size()):
			result[i][j] = m[i][j]
	return result
	
static func transpose(m: Array) -> Array:
	var result = make_matrix(m[0].size(), m.size())
	for i in range(m.size()):
		for j in range(m[0].size()):
			result[j][i] = m[i][j]
	return result
	
static func mul_mat(m1: Array, m2: Array) -> Array:
	assert(m1[0].size() == m2.size())
	var result = make_matrix(m1.size(), m2[0].size())
	for i in range(m1.size()):
		for j in range(m2[0].size()):
			result[i][j] = dot(extract_row(m1, i), extract_col(m2, j))
	return result
	
static func mul_vec(m: Array, v: Array) -> Array:
	assert(m[0].size() == v.size())
	var result = Array()
	result.resize(m.size())
	for i in range(m.size()):
		result[i] = dot(extract_row(m, i), v)
	return result
	
static func identity(n: int) -> Array:
	var result = make_matrix(n, n)
	for i in range(n):
		result[i][i] = 1
	return result
	
static func outer_product(v1: Array, v2: Array) -> Array:
	var result = make_matrix(v1.size(), v2.size())
	for i in range(v1.size()):
		for j in range(v2.size()):
			result[i][j] = v1[i] * v2[j]
	return result
	
static func diag(v: Array) -> Array:
	var result = make_matrix(v.size(), v.size())
	for i in range(v.size()):
		result[i][i] = v[i]
	return result

static func scale_add(m1: Array, m2: Array, s: float) -> Array:
	for i in range(m1.size()):
		for j in range(m1[0].size()):
			m1[i][j] += m2[i][j] * s
	return m1

static func is_zero(m: Array, eps: float=1e-4) -> bool:
	for row in m:
		for elem in row:
			if abs(elem) > eps:
				return false
	return true
