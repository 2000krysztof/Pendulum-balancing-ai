using Godot;
using System;

public partial class NumberLock : LineEdit
{
	public override void _Ready()
	{
		TextSubmitted += (string text) => {
			if(text.Equals("")){
					return;
			}
			try{
				int.Parse(text);
			}catch(Exception e){
				this.Text = "0";
			}
			
		};
	}


}
