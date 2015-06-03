using UnityEngine;
using System.Collections;

namespace WP.Controller
{
	public enum RAW_AXES_TYPE : ushort
	{
		UNKNOWN = 0,
		HORIZONTAL= 1,
		VERTICAL = 2,
	}
    
	public enum COMMAND_TYPE : ushort
	{
		UNKNOWN = 0,
		CAMERA_HORIZONTAL = 1,
		CAMERA_VERTICAL = 2,
		PLAYER_HORIZONTAL = 3,
		PLAYER_VERTICAL = 4,
		RESET_CAMERA = 5,
		CAMERA_ZOOM = 6,
		PLAYER_ROTATION = 7,
		PLAYER_ACTION = 8,
		CAMERA_CHANGE = 9,
		PLAYER_JUMP = 10,
		CAMERA_ROTATION = 11,
	}
    
	public class CommandFiredEventArgs
	{
		public ushort Command;
		public object[] Arguments = new object[0];
        
		public static CommandFiredEventArgs GenerateArgs (ushort command, params object[] args)
		{
			CommandFiredEventArgs newCommand = new CommandFiredEventArgs ();
			newCommand.Command = command;
			newCommand.Arguments = args;
			return newCommand;
		}
	}
    
	public delegate void ControllerCommandsFireHandler (System.Collections.Generic.List<CommandFiredEventArgs> commandList);
    
	public interface IController
	{
		event ControllerCommandsFireHandler OnControllerCommandsFired;
	}

	public interface IControlListener
	{
		void AddController (IController controller);
		void RemoveController (IController controller);
	}
    
	public static class CommandUtility
	{
		public static CommandFiredEventArgs SearchForCommandInList (this System.Collections.Generic.List<CommandFiredEventArgs> list, ushort commandType)
		{
			foreach (CommandFiredEventArgs eventArgs in list) {
				if (eventArgs.Command == commandType) {
					return eventArgs;
				}
			}
			return null;
		}
	}
   
}