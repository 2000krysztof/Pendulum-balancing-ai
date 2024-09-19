using Godot;
using System.Collections.Generic;
///<summary>
///Draws a chart of values added into it. Allows for automatic scaling of the graph based on the range of values added.
///</summary>
public partial class Chart : Node2D
{

	///<summary>
	///The list of all the values of the chart stored that are to be drawn
	///</summary>
	public List<float> Values {get; private set;}	
	[Export] float chartScale;



	public override void _Ready(){
		Values = new List<float>();	
	}

	///<summary>
	///Adds a new point to the graph and queues it up to be redrawn
	///</summary>
	///<param name="value">
	///The value of the new node that is to be added to the chart
	///</param>
	public void Add(float value){
		Values.Add(value);
		QueueRedraw();
	}

	///<summary>
	///Adds a new point to the graph and queues it up to be redrawn
	///</summary>
	///<param name="value">
	///The value of the new node that is to be added to the chart
	///</param>
	public void Add(double value){
		Values.Add((float)value);
		QueueRedraw();
	}

	// Draws the chart based on the largest value and scales them proportionally
	public override void _Draw(){
		float biggest = GetBiggest();
		Vector2 previousPosition = new Vector2(0,0);
		for(int i = 0; i<Values.Count;i++){
			Vector2 position = new Vector2(((float)i+1)/Values.Count,Values[i]/biggest)*chartScale;
			DrawCircle(position, 20,new Color(1,0.5f,0.5f));
			DrawLine(position, previousPosition, new Color(1,1,1),20f);
			previousPosition = position;
		}

	}

	//gets the bigest value in the list
	private float GetBiggest(){
		float biggest = 0;	
		foreach(float value in Values){
			if(value > biggest){
				biggest = value;
			}
		}
		return biggest;
	}
}
