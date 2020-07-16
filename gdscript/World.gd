extends Spatial

# Declare member variables here. Examples:
var Bee = preload("Bee.gd")
var Arm = preload("Arm.gd")
var Jt = preload("Joint.gd")
var Solver = preload("Solver.gd")

var endpoint: Jt
var bee: Bee
var joints: Array
var solver: Solver

# Called when the node enters the scene tree for the first time.
func _ready():
	
	bee = Bee.new()
	add_child(bee)
	
	var arm = Arm.new(null, 0)
	add_child(arm)
	joints = Array()
	joints.append(Jt.new(arm, Vector3.RIGHT))
	arm = Arm.new(joints.back(), 0)
	joints.append(Jt.new(arm, Vector3.FORWARD))
	arm = Arm.new(joints.back())
	joints.append(Jt.new(arm, Vector3.FORWARD))
	arm = Arm.new(joints.back())
	joints.append(Jt.new(arm, Vector3.RIGHT))
	arm = Arm.new(joints.back())
	endpoint = Jt.new(arm, Vector3.UP)
	var mat = SpatialMaterial.new()
	mat.albedo_color = Color.blue
	endpoint.mesh.material_override = mat
	solver = Solver.new(endpoint, joints)
	
	solver.test_svd([[1, 0], [2, 0]])
	solver.test_svd([[1, 2], [3, 4]])
	solver.test_svd([[1, 1], [1, 1]])
	
	solver.test_pseudoinverse([[1, 0], [2, 0]])
	solver.test_pseudoinverse([[1, 2], [3, 4]])
	solver.test_pseudoinverse([[1, 1], [1, 1]])
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	var scale = 5.0
	var drotation: Array = solver.compute_ik(bee.global_transform.origin, 0.2)
	for i in range(drotation.size()):
		var j: Jt = solver.joints[i]
		j.update_parameter(delta * drotation[i] * scale)
