using System;
using System.Linq;

namespace Quiz
{
    /// <summary>
    /// This class defines the structure of the answer for each question in the exam.
    /// </summary>
    public class Answer
    {
        public Student Student;
        public string Text;
        /// <summary>
        /// Constructror
        /// </summary>
        /// <param name="std"> The student who proposed this answer.</param>
        /// <param name="txt"> The text for the answer of the question.</param>
        public Answer(Student std, string txt = "")
        {
            this.Student = std;
            this.Text = txt;
        }
        /// <summary>
        /// Prepares a string format of this object to be logged.
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            string delimiter = " : ";
            return "Answer " + delimiter + this.Student.ToString() + delimiter + this.Text;
        }
    }
    /// <summary>
    /// Question to be designed by teachers for an exam and students are supposed to add their answer for each question.
    /// </summary>
    public class Question
    {
        public string Text { get; set; }
        public string TeacherCode;
        // Each question can collect answers by the students.
        public LinkedList<Answer> Answers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="txt"> The text of the question. </param>
        /// <param name="tcode"> The teacher code: It must be clear who was the designer of the question.</param>
        public Question(string txt, string tcode)
        {
            this.Text = txt;
            this.TeacherCode = tcode;
            this.Answers = new LinkedList<Answer>();
        }
        /// <summary>
        /// Adds the proposed answer to the question.
        /// </summary>
        /// <param name="a"> The answer proposed by the student will be added to the list of answers. </param>
        public virtual void AddAnswer(Answer a)
        {
            this.Answers.AddLast(a);
        }
        /// <summary>
        /// Prepares a string format of this object to be logged.
        /// </summary>
        /// <returns></returns>
        public String ToString()
        {
            string delim = " : ";
            return "Question Designed by: " + delim + this.TeacherCode;
        }
    }
    /// <summary>
    /// Defines the class for Students with name, stduent number, an exam, current question to be answered, current question number.
    /// </summary>
    public class Student
    {
        public string Name;
        public int Number;
        public LinkedListNode<Question>? Current;
        public Exam? Exam;
        public int CurrentQuestionNumber;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="num"> Number of the student</param>
        /// <param name="name"> Name of the student</param>
        public Student(int num, string name)
        {
            this.Name = name;
            this.Number = num;
            this.CurrentQuestionNumber = 0;
        }

        /// <summary>
        /// Assigns an exam to the student to be answered.
        /// </summary>
        /// <param name="e"></param>
        public virtual void AssignExam(Exam e)
        {
            this.Exam = e;
            this.Log("[Exam is Assigned]");
        }

        /// <summary>
        /// The student starts answering the exam: picks the question, thinks, proposes an answer and moves on to the next question.
        /// </summary>
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

        /// <summary>
        /// Thinking process for the students.
        /// </summary>
        public virtual void Think()
        {
            Thread.Sleep(new Random().Next(FixedParams.minThinkingTimeStudent, FixedParams.maxThinkingTimeStudent));
        }

        /// <summary>
        /// The student adds his/her answer to the current question and picks the next question.
        /// </summary>
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

        /// <summary>
        /// Prepares a string format of this object to be logged.
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// Defines the structur of Teachers with names, code and an exam to be designed.
    /// </summary>
    public class Teacher
    {
        public string Name;
        public string Code;
        public Exam? Exam;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Code of the teacher</param>
        /// <param name="name">Name of the teacher</param>
        public Teacher(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }
        /// <summary>
        /// An exam is assigned for the the teacher to add some questions.
        /// </summary>
        /// <param name="e">Exam to be designed</param>
        public virtual void AssignExam(Exam e)
        {
            this.Exam = e;
        }
        /// <summary>
        /// Thinking process of the teacher.
        /// </summary>
        public virtual void Think()
        {
            Thread.Sleep(new Random().Next(FixedParams.minThinkingTimeTeacher, FixedParams.maxThinkingTimeTeacher));
        }
        /// <summary>
        /// Teacher proposes and adds questions to the exam assigned to him/her.
        /// </summary>
        public virtual void ProposeQuestion()
        {
            this.Log("[Proposing Question]");

            string qtext = " [This is the text for Question] ";
            if (this.Exam is not null)
                this.Exam.AddQuestion(this, qtext);
        }
        /// <summary>
        /// Teacher starts with preparing the exam: thinks about a possible question and adds the question to the list.
        /// </summary>
        /// <param name="maxNumOfQuestions">Maximum number of the question to be designed.</param>
        public virtual void PrepareExam(int maxNumOfQuestions)
        {
            for (int i = 0; i < maxNumOfQuestions; i++)
            {
                this.Think();
                this.ProposeQuestion();
            }

        }
        /// <summary>
        /// Prepares a string format of this object to be logged.
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// The structure of the exam with a list of questions, total number of questions added, name of the course and number of the exam.
    /// </summary>
    public class Exam
    {
        public LinkedList<Question> Questions;
        private int Number;
        private string Name;
        private int QuestionNumber;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number"> A number defined for the exam</param>
        /// <param name="name">Name of the course to be examined</param>
        public Exam(int number, string name = "")
        {
            this.Questions = new LinkedList<Question>();
            this.QuestionNumber = 0;
            this.Name = name;
        }
        /// <summary>
        /// Adds a question to the list of the questions in the exam
        /// </summary>
        /// <param name="teacher">Designer of the question</param>
        /// <param name="text">Text of the question</param>
        public virtual void AddQuestion(Teacher teacher, string text)
        {
            this.QuestionNumber++;
            Question q = new Question(text, teacher.Code);
            this.Questions.AddLast(q);
            this.Log("[Question is added]" + q.ToString());
        }
        /// <summary>
        /// Prepares a string format of this object to be logged.
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// Defines the structure of the classroom in which an exam will take place with a list of students and teachers
    /// </summary>
    public class Classroom
    {
        public LinkedList<Student> Students;
        public LinkedList<Teacher> Teachers;
        public Exam Exam;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="examNumber">The Exam number to be taken</param>
        /// <param name="examName">Name of the exam</param>
        public Classroom(int examNumber = 1, string examName = "Programming")
        {
            this.Students = new LinkedList<Student>();
            this.Teachers = new LinkedList<Teacher>();
            this.Exam = new Exam(examNumber, examName); // only one exam
        }
        /// <summary>
        /// Prepares the set up needed for the exam: creates the list of the students and teachers, assigns the exam.
        /// </summary>
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
        /// <summary>
        /// Initiates teachers with preparing the exam
        /// </summary>
        /// <param name="maxNumOfQuestion"></param>
        public virtual void PrepareExam(int maxNumOfQuestion)
        {
            foreach (Teacher t in this.Teachers)
                t.PrepareExam(maxNumOfQuestion);
        }
        /// <summary>
        /// Distributes the exam among the students
        /// </summary>
        public virtual void DistributeExam()
        {
            foreach (Student s in this.Students)
                s.AssignExam(this.Exam);
        }
        /// <summary>
        /// Students start the exam
        /// </summary>
        public virtual void StartExams()
        {
            foreach (Student s in this.Students)
                s.StartExam();
        }
        /// <summary>
        /// Prepares some information about the exams taken in the classroom
        /// </summary>
        /// <returns>Result of the exams as a string</returns>
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
    /// <summary>
    /// A class to run the examination in sequentially.
    /// </summary>
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

