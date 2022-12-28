using System;
using System.IO;
using System.Diagnostics;
//using ConcQuiz; //this has no implementation yet, fill it in and then uncomment this :)

namespace Quiz
{
    // The values within WorkingParams can change during the experiments. 
    class SubmissionParams
    {
        public const string studentNumberOne = ""; // This must be filled.
        public const string studentNumberTwo = ""; // This must be filled. Keep it "" if you are working alone.
        public const string classNumber = ""; // This must be filled. INF2A is just an example.
    }

    // The values of FixedParams must not change in the final submission.
    class FixedParams
    {
        public const int minThinkingTimeTeacher = 50;
        public const int maxThinkingTimeTeacher = 200;
        public const int minThinkingTimeStudent = 20;
        public const int maxThinkingTimeStudent = 100;
        public const int maxNumOfQuestions = 40; 
        public const int maxNumOfStudents = 200;
        public const int maxNumOfTeachers = 10;
        public const char delim = ',';
    }

    class Program
    {
        static void Main(string[] args)
        {
            string logFooter = "", logSeqContent = " Sequential Run: \n", logConcContent = " Concurrent Run: \n", logTiming = "";

            Stopwatch seqSW = new Stopwatch();
            Stopwatch conSW = new Stopwatch();

            seqSW.Start();
            QuizSequential sq = new QuizSequential();
            sq.RunExams();
            logSeqContent = logSeqContent + sq.FinalResult();
            seqSW.Stop();

            TimeSpan seqET = seqSW.Elapsed;

            conSW.Start();
            //QuizConcurrent cq = new QuizConcurrent();
            //cq.RunExams();
            //logConcContent = logConcContent + cq.FinalResult();
            conSW.Stop();

            TimeSpan conET = conSW.Elapsed;

            logTiming =
                "Time Sequential = " + seqET.Minutes + " min, " + seqET.Seconds + "sec, " + seqET.Milliseconds + " msec. " + "\n" +
                "Time Concurrent = " + conET.Minutes + " min, " + conET.Seconds + "sec, " + conET.Milliseconds + " msec. " + "\n";

            logFooter =
                "Number of Students: " + FixedParams.maxNumOfStudents + "\n" +
                "Number of Teachers: " + FixedParams.maxNumOfTeachers + "\n" +
                "Number of Questions: " + FixedParams.maxNumOfTeachers*FixedParams.maxNumOfQuestions + "\n" +
                "Class: " + SubmissionParams.classNumber + "\n" +
                "Student Number One: " + SubmissionParams.studentNumberOne + "\n" +
                "Student Number Two: " + SubmissionParams.studentNumberTwo + "\n";

            
            Console.WriteLine("----------------");
            Console.WriteLine(logSeqContent);
            Console.WriteLine("----------------");
            Console.WriteLine(logConcContent);
            Console.WriteLine("----------------");
            Console.WriteLine(logFooter);
            Console.WriteLine(logTiming);
        }
    }
}
