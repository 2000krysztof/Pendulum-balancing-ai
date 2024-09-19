using Godot;

public partial class Brain : Node
{
	public NeuralNetwork network {get;  set;}
	
	RigidBody2D rigidBody;
	StaticBody2D staticBody;

	[Export] float speed;
	[Export] float upperBound, lowerBound;

	[Export(PropertyHint.Range, "0,1,0.01")] double connectionProbability;
	public override void _Ready()
	{
		rigidBody = GetNode<RigidBody2D>("../Pendulum");
		staticBody = GetNode<StaticBody2D>("../Base");
		
		network = new NeuralNetwork(3,1,connectionProbability); 
	}

    public override void _Process(double delta)
    {
		double[] result = network.Run(new double[]{rigidBody.AngularVelocity,(double)rigidBody.Position.X, (double)rigidBody.Position.Y });
		
		staticBody.Translate(new Vector2((float)(delta*speed*result[0]),0));

		if(staticBody.Position.X <lowerBound){staticBody.Position = new Vector2(lowerBound,staticBody.Position.Y);}
		if(staticBody.Position.X >upperBound){staticBody.Position = new Vector2(upperBound,staticBody.Position.Y);} 
    }
	
	public void Mutate(){
		network.Mutate();
	}



}
