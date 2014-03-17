using SchemaUpdate.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaUpdate.Structs
{
    internal struct EntityTableLink
    {
        public readonly Model ConceptualModel;

        public readonly Model PersistantModel;

        public EntityTableLink(Model conceptualModel, Model persistantModel)
        {
            this.ConceptualModel = conceptualModel;
            this.PersistantModel = persistantModel;
        }
    }
}
