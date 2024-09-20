using Godot;
using System;

public partial class SliderUpdate : HSlider
{
	[Export] Label valueLabel;
	public override void _Ready()
	{
		ValueChanged += (double value) => {
			valueLabel.Text = $"{value}";
		};
	}

}
