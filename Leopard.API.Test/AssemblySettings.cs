using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

//You can only have one of these attributes per assembly; 
// if you want to combine multiple behaviors, do it with a single attribute.
// Also be aware that these values affect only this assembly;
// if multiple assemblies are running in parallel against one another, they have their own independent values.
[assembly: CollectionBehavior(MaxParallelThreads = 9)]
//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace Leopard.API.Test
{
}
