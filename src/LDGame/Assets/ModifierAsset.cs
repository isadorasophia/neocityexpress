using LDGame.Core;
using Murder.Assets;
using System.Numerics;

namespace LDGame.Assets;

public class ModifierAsset : GameAsset
{
    public override char Icon => '';
    public override string EditorFolder => "#Modifiers";
    public override Vector4 EditorColor => new Vector4(.15f, 1, .5f, 1f);

    public readonly Modifier Modifier;

    public readonly float PhoneSway;

    public readonly bool Sleepy = false;
}