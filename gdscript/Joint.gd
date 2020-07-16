extends Spatial

class_name Jt

# Declare member variables here. Examples:
var parent
var rotationAxis: Vector3
var rotationParameter: float
var savedParameter: float
var mesh: MeshInstance

func _init(p=null, ax=Vector3.RIGHT):
	parent = p
	rotationAxis = ax
	var sphere = SphereMesh.new()
	sphere.radius = 0.2
	sphere.height = 0.4
	mesh = MeshInstance.new()
	mesh.mesh = sphere
	add_child(mesh)
	parent.add_child(self)
	set_parameter(0.5)

func set_parameter(param: float):
	rotationParameter = param
	transform = Transform(
		Quat(rotationAxis, rotationParameter),
		parent.get_endpoint())

func update_parameter(dtheta: float):
	set_parameter(rotationParameter + dtheta)

func save_state():
	savedParameter = rotationParameter
	
func reset_state():
	set_parameter(savedParameter)
