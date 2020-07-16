extends Spatial
class_name Arm
# Declare member variables here. Examples:
var Jt = preload("Joint.gd")
var parent: Jt
var length: float
var mesh: MeshInstance
	
func _init(p: Jt=null, leng: float=2.0):
	parent = p
	length = leng
	if parent != null:
		parent.add_child(self)
	var cylinder = CylinderMesh.new()
	cylinder.height = length
	cylinder.top_radius = 0.1
	cylinder.bottom_radius = 0.1
	mesh = MeshInstance.new()
	mesh.mesh = cylinder
	mesh.translation = Vector3.UP * length / 2
	var mat = SpatialMaterial.new()
	mat.albedo_color = Color.gray
	mat.metallic = 0.5
	mesh.material_override = mat
	add_child(mesh)

func get_endpoint():
	return Vector3.UP * length
