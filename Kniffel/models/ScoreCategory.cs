namespace Kniffel.models;

public class ScoreCategory
    {
        public string Name { get; set; }
        public int? Score { get; set; } 
        public bool IsUsed { get; set; }
        public bool IsSelected { get; set; } = false;
    }


    