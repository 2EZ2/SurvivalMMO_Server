using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    public static class UniqueIDGenerator
    {
        public static HashSet<Guid> UsedUniqueIDs = new HashSet<Guid>();

        public static Guid generateID()
        {
            Guid newId = Guid.NewGuid();

            lock (UsedUniqueIDs)
            {
                UsedUniqueIDs.Add(newId);
            }

            return newId;
        }

        public static Guid[] generateIDs(int count)
        {
            List<Guid> newId = new List<Guid>();
            Guid tempID;

            lock (UsedUniqueIDs)
            {
                while(newId.Count != count)
                {
                    tempID = Guid.NewGuid();
                    if (!UsedUniqueIDs.Contains(tempID))
                    {
                        newId.Add(tempID);
                        UsedUniqueIDs.Add(tempID);                        
                    }                   
                }
            }
            
            return newId.ToArray();
        }

        public static void freeID(Guid id)
        {
            lock(UsedUniqueIDs)
            {
                if (UsedUniqueIDs.Contains(id))
                {
                    UsedUniqueIDs.Remove(id);
                }                
            }
        }

        public static void freeIDs(Guid[] ids)
        {
            lock (UsedUniqueIDs)
            {
                foreach (Guid id in ids)
                {
                    if (UsedUniqueIDs.Contains(id))
                    {
                        UsedUniqueIDs.Remove(id);
                    }
                }                
            }
        }
    }
}
