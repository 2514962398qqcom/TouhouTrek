using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMDFQ
{
    public interface IDatabase
    {
        int getID(Type cardType);
        Type getType(int configID);
        Card GetCard(int configId);
        Skill GetSkill(int configId);
    }
}
