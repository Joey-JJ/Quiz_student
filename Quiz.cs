using System;
using System.Linq;

namespace Quiz
{

    public class Answer
    {
        public Student Student;
        public string Text;

        public Answer(Student std, string txt = "")
        {
            this.Student = std;
            this.Text = txt;
        }

        public string ToString()
        {
            string delimiter = " : ";
            return "Answer " + delimiter + this.Student.ToString() + delimiter + this.Text;
        }
    }

    public class Question
    {
        public string Text { get; set; }
        public string TeacherCode;
        // Each question can collect answers by the students.
        public LinkedList<Answer> Answers;
        public Question(string txt, string tcode)
        {
            this.Text = txt;
            this.TeacherCode = tcode;
            this.Answers = new LinkedList<Answer>();
        }
        public virtual void AddAnswer(Answer a)
        {
            this.Answers.AddLast(a);
        }

        public String ToString()
        {
            string delim = " : ";
            return "Question Designed by: " + delim + this.TeacherCode;
        }
    }

    public class Student
    {
        public string Name;
        public int Number;
        public LinkedListNode<Question>? Current;
        public Exam? Exam;
        public int CurrentQuestionNumber;

        public Student(int num, string name)
        {
            this.Name = name;
            this.Number = num;
            this.CurrentQuestionNumber = 0;
        }

        public virtual void AssignExam(Exam e)
        {
            this.Exam = e;
            this.Log("[Exam is Assigned]");
        }

        public virtual void StartExam()
        {
            if (this.Exam is not null)
                this.Current = this.Exam.Questions.First;
            for (int i = 0; i < this.Exam.Questions.Count; i++)
            {
                this.Think();
                this.ProposeAnswer();
            }
        }

        public virtual void Think()
        {
            Thread.Sleep(new Random().Next(FixedParams.minThinkingTimeStudent, FixedParams.maxThinkingTimeStudent));
        }

        public virtual void ProposeAnswer()
        {
            if (this.Current is not null)
            {
                this.Log("\n[Proposing Answer]\n");
                // add your answer
                this.Current.Value.AddAnswer(new Answer(this));
                // go for the next question
                this.Current = this.Current.Next;
                this.CurrentQuestionNumber++;
            }
        }

        public string ToString()
        {
            string delim = " : ", nl = "\n";
            return "Student " + delim + this.Number.ToString() + nl + delim + this.Exam.ToString() + delim + "Current Question: " + this.CurrentQuestionNumber.ToString();
        }

        public virtual void Log(string logText = "")
        {
            string nl = "\n";
            Console.WriteLine(logText + nl + this.ToString());
        }

    }

    public class Teacher
    {
        public string Name;
        public string Code;
        public Exam? Exam;

        public Teacher(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }

        public virtual void AssignExam(Exam e)
        {
            this.Exam = e;
        }

        public virtual void Think()
        {
            Thread.Sleep(new Random().Next(FixedParams.minThinkingTimeTeacher, FixedParams.maxThinkingTimeTeacher));
        }

        public virtual void ProposeQuestion()
        {
            this.Log("[Proposing Question]");

            string qtext = " [This is the text for Question] ";
            if (this.Exam is not null)
                this.Exam.AddQuestion(this, qtext);
        }
        public virtual void PrepareExam(int maxNumOfQuestions)
        {
            for (int i = 0; i < maxNumOfQuestions; i++)
            {
                this.Think();
                this.ProposeQuestion();
            }

        }

        public string ToString()
        {
            string delim = " : ", nl = "\n";
            return "Teacher " + delim + this.Name + nl + " Code " + delim + this.Code;
        }

        public virtual void Log(string logText = "")
        {
            string nl = "\n";
            Console.WriteLine(this.ToString() + nl + logText);
        }
    }

    public class Exam
    {
        public LinkedList<Question> Questions;
        private int Number;
        private string Name;
        private int QuestionNumber;

        public Exam(int number, string name = "")
        {
            this.Questions = new LinkedList<Question>();
            this.QuestionNumber = 0;
            this.Name = name;
        }

        public virtual void AddQuestion(Teacher teacher, string text)
        {
            this.QuestionNumber++;
            Question q = new Question(text, teacher.Code);
            this.Questions.AddLast(q);
            this.Log("[Question is added]" + q.ToString());
        }

        public override string ToString()
        {
            string delim = " : ", nl = "\n";
            return "Exam " + delim + this.Number.ToString() + delim + " Total Num Questions: " + this.QuestionNumber.ToString();
        }
        public virtual void Log(string logText = "")
        {
            string nl = "\n";
            Console.WriteLine(this.ToString() + nl + logText);
        }
    }

    public class Classroom
    {
        public LinkedList<Student> Students;
        public LinkedList<Teacher> Teachers;
        public Exam Exam;
        public Classroom(int examNumber = 1, string examName = "Programming")
        {
            this.Students = new LinkedList<Student>();
            this.Teachers = new LinkedList<Teacher>();
            this.Exam = new Exam(examNumber, examName); // only one exam
        }

        public virtual void SetUp()
        {
            for (int i = 0; i < FixedParams.maxNumOfStudents; i++)
            {
                string std_name = " STUDENT NAME"; //todo: to be generated later
                this.Students.AddLast(new Student(i + 1, std_name));
            }
            for (int i = 0; i < FixedParams.maxNumOfTeachers; i++)
            {
                string teacher_name = " TEACHER NAME"; //todo: to be generated later
                this.Teachers.AddLast(new Teacher((i + 1).ToString(), teacher_name));
            }
            // assign exams
            foreach (Teacher t in this.Teachers)
                t.AssignExam(this.Exam);
        }

        public virtual void PrepareExam(int maxNumOfQuestion)
        {
            foreach (Teacher t in this.Teachers)
                t.PrepareExam(maxNumOfQuestion);
        }

        public virtual void DistributeExam()
        {
            foreach (Student s in this.Students)
                s.AssignExam(this.Exam);
        }

        public virtual void StartExams()
        {
            foreach (Student s in this.Students)
                s.StartExam();
        }

        public string GetStatistics()
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

    public class QuizSequential
    {
        Classroom classroom;

        public QuizSequential()
        {
            this.classroom = new Classroom();
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

