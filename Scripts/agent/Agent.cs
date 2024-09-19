using Godot;

public partial class Agent : Node2D{

	[Export]Brain brain;
	[Export]RigidBody2D pendulumHead;
	[Export]StaticBody2D penulumBase;
	[Export] float margin;
	public double score {get; private set;}
	float lenght;
	public double fitness{get; private set;}
	public override void _Ready(){
		lenght = pendulumHead.Position.DistanceTo(penulumBase.Position)-margin;
	}

	public override void _Process(double delta){
		FitnessFunction(delta);	 
	}

	public void Mutate(){
		brain.Mutate();
	}

	void FitnessFunction(double delta){

		if(penulumBase.Position.Y - pendulumHead.Position.Y > lenght){
			fitness += delta; 
		}
		if(pendulumHead.Position.Y- penulumBase.Position.Y > lenght){
			fitness -= delta;
		}
	}
	
}
