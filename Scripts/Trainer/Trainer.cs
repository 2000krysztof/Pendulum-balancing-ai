using Godot;
using System;

public partial class Trainer : Node
{
	PackedScene agent = GD.Load<PackedScene>("res://Scenes/agent.tscn");
	[Export] Timer timer;
	[Export] Vector2 spawnPostion;
	[Export] int batchSize = 10;
	[Export(PropertyHint.Range, "0,1,0.01")] float percentageToMutate;
	Agent[] agents;
	int mutationCount, generation;

	[Signal] public delegate void OnSetBestEventHandler(Agent agent);
	[Export] Chart chart;

	public static Trainer instance;
	public override void _Ready()
	{
		instance = this;
		generation = 0;
		agents = new Agent[batchSize];
		for(int i = 0; i < batchSize; i++){
			Agent instance = (Agent)agent.Instantiate();
			instance.Position = spawnPostion;
			AddChild(instance);
			agents[i] = instance;
		}
		timer.Timeout += OnTimeOut;
		mutationCount = (int)(batchSize*percentageToMutate);

	}

	void OnTimeOut(){
		GD.Print($"Generation : {generation} | Average fitness : {AverageFitness()}");
		generation ++;
		SortAgentsByFitness();	
		GD.Print($"Average Fitnees of Best: {AverageFitnessOfBest()}");
		chart.Add(AverageFitnessOfBest());
		MutateWorst();
		ResetAgents();
		EmitSignal(SignalName.OnSetBest, GetBestAgent());
	}


	void SortAgentsByFitness(){
		Array.Sort(agents, (b,a) => b.fitness.CompareTo(a.fitness));
	}

	void MutateWorst(){
		for(int i = mutationCount-1; i>-1;i--){
			agents[i].Mutate();
		}
	}

	void ResetAgents(){
		for(int i =0; i<agents.Length; i++){
			Agent newAgent = (Agent)agent.Instantiate();
			AddChild(newAgent);
			Brain oldBrain = agents[i].GetNode<Brain>("Brain:Brain");
			Brain newBrain = newAgent.GetNode<Brain>("Brain:Brain");
			newBrain.network = oldBrain.network;
			agents[i].QueueFree();
			agents[i] = newAgent;
			newAgent.Position = spawnPostion;
		}
	}

	///<returns>
	/// The average fitness of all the agents in the batch
	///</returns>
	///<summary>
	///Useful for determining what agents are preforming well compared to others
	///</summary>
	public double AverageFitness(){
		double average = 0;
		foreach(Agent agent in agents){
			average = agent.fitness;
		}
		average/= agents.Length;
		return average;

	}


	///<returns>
	/// The average fitness of the agents who will go to the next batch.
	///</returns>
	///<summary>
	///Useful for monitoring the progress of learning
	///</summary>
	public double AverageFitnessOfBest(){
		double average = 0;
		for(int i = mutationCount; i < agents.Length; i++){
			average += agents[i].fitness;
		}
		return average/= agents.Length-mutationCount;
	}	


	///<returns>
	///The best agent in the batch
	///</returns>
	public Agent GetBestAgent(){
		//the first agent is returned because agents are sorted by fitness anyway
		return agents[0];
	}

	private void _on_show_best_button_pressed(){
		GD.Print("Pressed");
	}
	
	private void _on_save_best_button_pressed(){
		Brain brain = GetBestAgent().GetNode<Brain>("Brain:Brain");
		//TODO save method to be completed
	}
}




