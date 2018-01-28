using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FairyLink.Actor
{
    interface ICharacterMediator
    {
        void AddCharacter(Character character);
        Vector2 GetPlayerPosition();
    }
}
