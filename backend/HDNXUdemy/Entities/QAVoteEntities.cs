namespace HDNXUdemyData.Entities
{
    public class QAVoteEntities : BaseEntities
    {
        public long IdQuestion { get; set; } // TheadQuestionCourse or DetailTheadQuestionCourse
        public long IdUser { get; set; }
        public bool IsUpvote { get; set; } // true = upvote, false = downvote
        public bool IsForAnswer { get; set; } // true = vote on answer, false = vote on question
    }
}
