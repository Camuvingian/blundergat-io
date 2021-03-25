using Blundergat.Common.Types;

namespace Blundergat.Common.Settings.Interfaces
{
	public interface ISetting<T> : IType
	{
		string Command { get; set; }
		string Description { get; set; }
		T Value { get; set; }
	}
}