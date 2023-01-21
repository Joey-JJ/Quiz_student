using System;
using Quiz;

// todo: Complete the implementation

/// Concurrent version of the Quiz
namespace ConcQuiz
{
    public class ConcAnswer : Answer
    {
        public ConcAnswer(ConcStudent std, string txt = "") : base(std, txt) { }
    }

    public class ConcQuestion : Question
    {
        //todo: add required fields, if necessary
        public LinkedList<ConcAnswer> ConcAnswers;
        private readonly object _lock = new object();

        public ConcQuestion(string txt, string tcode) : base(txt, tcode)
        {
            this.ConcAnswers = new LinkedList<ConcAnswer>();
        }

        public override void AddAnswer(Answer a)
        {
            //todo: implement the body 
            lock (_lock)
            {
                this.ConcAnswers.AddLast((ConcAnswer)a);
            }
        }
    }

    public class ConcStudent : Student
    {
        // todo: add required fields
        public LinkedListNode<ConcQuestion>? ConcCurrent;
        public ConcExam? ConcExam;

        public ConcStudent(int num, string name) : base(num, name) { }

        public override void AssignExam(Exam e)
        {
            //todo: implement the body
            this.ConcExam = (ConcExam)e;
            this.Log("[Exam is Assigned]");
        }

        public override void StartExam()
        {
            //todo: implement the body
            if (this.ConcExam is not null)
            {
                this.ConcCurrent = this.ConcExam.ConcQuestions.First;
                for (int i = 0; i < this.ConcExam.ConcQuestions.Count; i++)
                {
                    this.Think();
                    this.ProposeAnswer();
                }
            }
        }

        public override void Think()
        {
            //todo: implement the body
            Thread.Sleep(new Random().Next(FixedParams.minThinkingTimeStudent, FixedParams.maxThinkingTimeStudent));
        }

        public override void ProposeAnswer()
        {
            //todo: implement the body
            if (this.ConcCurrent is not null)
            {
                this.Log("\n[Proposing Answer]\n");
                // add your answer
                this.ConcCurrent.Value.AddAnswer(new ConcAnswer(this));
                // go for the next question
                this.ConcCurrent = this.ConcCurrent.Next;
                this.CurrentQuestionNumber++;
            }
        }

        public new string ToString()
        {
            string delim = " : ", nl = "\n";
            return "Student " + delim + this.Number.ToString() + nl + delim + this.ConcExam?.ToString() + delim + "Current Question: " + this.CurrentQuestionNumber.ToString();
        }

        public override void Log(string logText = "")
        {
            string nl = "\n";
            Console.WriteLine(logText + nl + this.ToString());
        }

    }
    public class ConcTeacher : Teacher
    {
        //todo: add required fields, if necessary
        public ConcExam? ConcExam;

        public ConcTeacher(string code, string name) : base(code, name) { }

        public void AssignExam(ConcExam e)
        {
            //todo: implement the body
            this.ConcExam = e;
        }

        public override void Think()
        {
            //todo: implement the body
            Thread.Sleep(new Random().Next(FixedParams.minThinkingTimeTeacher, FixedParams.maxThinkingTimeTeacher));
        }

        public override void ProposeQuestion()
        {
            //todo: implement the body
            this.Log("[Proposing Question]");

            string qtext = " [This is the text for Question] ";
            if (this.ConcExam is not null)
                this.ConcExam.AddQuestion(this, qtext);
        }

        public override void PrepareExam(int maxNumOfQuestions)
        {
            //todo: implement the body
            for (int i = 0; i < maxNumOfQuestions; i++)
            {
                this.Think();
                this.ProposeQuestion();
            }
        }

        public new string ToString()
        {
            string delim = " : ", nl = "\n";
            return "Teacher " + delim + this.Name + nl + " Code " + delim + this.Code;
        }


        public override void Log(string logText = "")
        {
            string nl = "\n";
            Console.WriteLine(this.ToString() + nl + logText);
        }
    }
    public class ConcExam : Exam
    {
        //todo: add required fields, if necessary
        public LinkedList<ConcQuestion> ConcQuestions;
        private int Number;
        private int QuestionNumber;
        // lock obj
        private readonly object _lock = new object();


        public ConcExam(int number, string name = "") : base(number, name)
        {
            this.ConcQuestions = new LinkedList<ConcQuestion>();
            this.QuestionNumber = 0;
            this.Number = number;
        }

        public void AddQuestion(ConcTeacher teacher, string text)
        {
            //todo: implement the body
            lock (_lock)
            {
                this.QuestionNumber++;
                ConcQuestion q = new ConcQuestion(text, teacher.Code);
                this.ConcQuestions.AddLast(q);
                this.Log("[Question is added]" + q.ToString());
            }
        }

        public new string ToString()
        {
            string delim = " : ";
            return "Exam " + delim + this.Number.ToString() + delim + " Total Num Questions: " + this.QuestionNumber.ToString();
        }

        public override void Log(string logText = "")
        {
            string nl = "\n";
            Console.WriteLine(this.ToString() + nl + logText);
        }
    }

    public class ConcClassroom : Classroom
    {
        //todo: add required fields, if necessary
        public LinkedList<ConcStudent> ConcStudents;
        public LinkedList<ConcTeacher> ConcTeachers;
        public ConcExam ConcurrentExam { get; set; }

        public ConcClassroom(int examNumber = 1, string examName = "Programming") : base(examNumber, examName)
        {
            //todo: implement the body
            this.ConcStudents = new LinkedList<ConcStudent>();
            this.ConcTeachers = new LinkedList<ConcTeacher>();
            this.ConcurrentExam = new ConcExam(examNumber, examName);
        }

        public override void SetUp()
        {
            //todo: implement the body
            var t1 = new Thread(() =>
            {
                for (int i = 0; i < FixedParams.maxNumOfStudents; i++)
                {
                    System.Console.WriteLine("Creating student");
                    string std_name = $"STUDENT NAME {i}"; //todo: to be generated later
                    this.ConcStudents.AddLast(new ConcStudent(i + 1, std_name));
                }
            });

            var t2 = new Thread(() =>
            {
                for (int i = 0; i < FixedParams.maxNumOfTeachers; i++)
                {
                    string teacher_name = $"TEACHER NAME {i}"; //todo: to be generated later
                    this.ConcTeachers.AddLast(new ConcTeacher((i + 1).ToString(), teacher_name));
                }
            });

            t1.Start();
            t2.Start();

            // assign exams
            t1.Join();
            t2.Join();
            foreach (ConcTeacher t in this.ConcTeachers)
                t.AssignExam(this.ConcurrentExam);
        }

        public override void PrepareExam(int maxNumOfQuestion)
        {
            //todo: implement the body
            List<Thread> threads = new List<Thread>();
            foreach (ConcTeacher t in this.ConcTeachers)
            {
                var x = new Thread(() => t.PrepareExam(maxNumOfQuestion));
                x.Start();
                threads.Add(x);
            }

            threads.ForEach(x => x.Join());
        }

        public override void DistributeExam()
        {
            //todo: implement the body
            foreach (ConcStudent s in this.ConcStudents)
                s.AssignExam(this.ConcurrentExam);
        }

        public override void StartExams()
        {
            //todo: implement the body
            List<Thread> threads = new List<Thread>();
            foreach (ConcStudent s in this.ConcStudents)
            {
                var t = new Thread(() => s.StartExam());
                t.Start();
                threads.Add(t);
            }
            threads.ForEach(x => x.Join());
        }

        public new string GetStatistics()
        {
            string result = "", nl = "\n";
            int totalNumOfAnswers = 0;
            foreach (ConcQuestion q in this.ConcurrentExam.ConcQuestions)
                totalNumOfAnswers += q.ConcAnswers.Count;
            result = "#Students: " + this.ConcStudents.Count.ToString() + nl +
                "#Teachers: " + this.ConcTeachers.Count.ToString() + nl +
                "#Questions: " + this.ConcurrentExam.ConcQuestions.Count.ToString() + nl +
                "#Answers: " + totalNumOfAnswers.ToString();
            return result;
        }
    }
    //THIS CLASS (QUIZCONCURRENT) SHOULD NOT BE CHANGED
    public class QuizConcurrent
    {
        ConcClassroom classroom;

        public QuizConcurrent()
        {
            this.classroom = new ConcClassroom();
        }
        public void RunExams()
        {
            classroom.SetUp();
            classroom.PrepareExam(Quiz.FixedParams.maxNumOfQuestions);
            classroom.DistributeExam();
            classroom.StartExams();
        }
        public string FinalResult()
        {
            return classroom.GetStatistics();
        }

    }
}

