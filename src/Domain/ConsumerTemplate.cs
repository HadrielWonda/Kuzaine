using System.Collections.Generic;



namespace Kuzaine.Domain;

public class ConsumerTemplate
{
    public string SolutionName { get; set; }
    public List<Consumer> Consumers { get; set; }
}
