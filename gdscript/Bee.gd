extends Spatial

class_name Bee

# Declare member variables here. Examples:
var target: Vector3
var position: Vector3
var mesh: MeshInstance
var timer: Timer


func generate_random_position():
	var rg = 5
	return Vector3(
		2 * rg * randf() - rg,
		2 * rg * randf() - rg,
		2 * rg * randf() - rg
	)

# Called when the node enters the scene tree for the first time.
func _ready():
	target = generate_random_position()
	var sphere = SphereMesh.new()
	sphere.radius = 0.3
	sphere.height = 0.6
	mesh = MeshInstance.new()
	mesh.mesh = sphere
	var mat = SpatialMaterial.new()
	mat.albedo_color = Color.red
	mesh.material_override = mat
	add_child(mesh)
	
	timer = Timer.new()
	add_child(timer)
	var _res = timer.connect("timeout", self, "on_timeout")
	timer.start(4)

func on_timeout():
	target = generate_random_position()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	var speed = 10
	var dir = target - position
	if dir.length_squared() > 1:
		dir = dir.normalized()
	position += dir * speed * delta
	var new_transform = Transform(Basis.IDENTITY, position)
	global_transform = new_transform
