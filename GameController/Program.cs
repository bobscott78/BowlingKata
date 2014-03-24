using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using BowlingKata;

namespace GameController
{
    class Program
    {
        static void Main(string[] args)
        {
            var finished = false;
            var scores = Observable.Create<int>(o =>
                {
                    while (!finished)
                    {
                        var readLine = Console.ReadLine();
                        if (readLine == "END")
                        {
                            finished = true;
                        }
                        else
                        {
                            o.OnNext(int.Parse(readLine));
                        }
                    }
                    o.OnCompleted();
                    return Disposable.Empty;
                });

            var game = new Game(scores);
            Console.WriteLine(game.Score());
            Console.ReadLine();
        }
    }
}
