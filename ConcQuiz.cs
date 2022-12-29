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
        object lockObj = new object();

        public ConcQuestion(string txt, string tcode) : base(txt, tcode) { }

        public override void AddAnswer(Answer a)
        {
            //todo: implement the body 
            lock (lockObj)
            {
                this.Answers.AddLast(a);
            }
        }
    }

    public class ConcStudent : Student
    {
        // todo: add required fields

        public ConcStudent(int num, string name) : base(num, name) { }

        // public override void AssignExam(Exam e)
        // {
        //     //todo: implement the body
        // }

        // public override void StartExam()
        // {
        //     //todo: implement the body
        // }

        // public override void Think()
        // {
        //     //todo: implement the body
        // }

        // public override void ProposeAnswer()
        // {
        //     //todo: implement the body
        // }

        public override void Log(string logText = "")
        {
            base.Log();
        }

    }
    public class ConcTeacher : Teacher
    {
        //todo: add required fields, if necessary
        // lockobj
        object lockObj = new object();

        public ConcTeacher(string code, string name) : base(code, name) { }

        // public override void AssignExam(Exam e)
        // {
        //     //todo: implement the body
        // }
        // public override void Think()
        // {
        //     //todo: implement the body
        // }
        public override void ProposeQuestion()
        {
            //todo: implement the body
            lock (lockObj)
            {
                base.ProposeQuestion();
            }

        }
        // public override void PrepareExam(int maxNumOfQuestions)
        // {
        //     //todo: implement the body
        // }
        public override void Log(string logText = "")
        {
            base.Log();
        }
    }
    public class ConcExam : Exam
    {
        //todo: add required fields, if necessary
        object lockObj = new object();

        public ConcExam(int number, string name = "") : base(number, name) { }

        public override void AddQuestion(Teacher teacher, string text)
        {
            //todo: implement the body
            base.AddQuestion(teacher, text);

        }
        public override void Log(string logText = "")
        {
            base.Log();
        }
    }

    public class ConcClassroom : Classroom
    {
        //todo: add required fields, if necessary

        public ConcClassroom(int examNumber = 1, string examName = "Programming") : base(examNumber, examName)
        {
            //todo: implement the body
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
                    this.Students.AddLast(new Student(i + 1, std_name));
                }
            });

            var t2 = new Thread(() =>
            {
                for (int i = 0; i < FixedParams.maxNumOfTeachers; i++)
                {
                    string teacher_name = $"TEACHER NAME {i}"; //todo: to be generated later
                    this.Teachers.AddLast(new Teacher((i + 1).ToString(), teacher_name));
                }
            });

            t1.Start();
            t2.Start();

            // assign exams
            t1.Join();
            t2.Join();
            foreach (Teacher t in this.Teachers)
                t.AssignExam(this.Exam);
        }

        public override void PrepareExam(int maxNumOfQuestion)
        {
            //todo: implement the body
            List<Thread> threads = new List<Thread>();
            foreach (Teacher t in this.Teachers)
            {
                var x = new Thread(() => t.PrepareExam(maxNumOfQuestion));
                x.Start();
                threads.Add(x);
            }

            threads.ForEach(x => x.Join());
        }

        // public override void DistributeExam()
        // {
        //     //todo: implement the body
        // }

        public override void StartExams()
        {
            //todo: implement the body
            List<Thread> threads = new List<Thread>();
            foreach (Student s in this.Students)
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
            foreach (Question q in this.Exam.Questions)
                totalNumOfAnswers += q.Answers.Count;
            result = "#Students: " + this.Students.Count.ToString() + nl +
                "#Teachers: " + this.Teachers.Count.ToString() + nl +
                "#Questions: " + this.Exam.Questions.Count.ToString() + nl +
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

