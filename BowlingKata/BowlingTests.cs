using System;
using System.Linq;
using System.Reactive.Linq;
using NUnit.Framework;

namespace BowlingKata
{
    [TestFixture]
    public class BowlingTests
    {
        [Test]
        public void NormalFrame()
        {
            var frames = "2,3".Split(',').Select(int.Parse).ToObservable();
            
            var game = new Game(frames);

            Assert.That(game.Score(), Is.EqualTo(5));
        }

        [Test]
        public void SpareFrameScoresNextDouble()
        {
            var frames = "9,1,7,0".Split(',').Select(int.Parse).ToObservable();

            var game = new Game(frames);

            Assert.That(game.Score(), Is.EqualTo(24));
        }

        [Test]
        public void TwoSparesFrameScoresNextDouble()
        {
            var frames = "9,1,7,3,2,0".Split(',').Select(int.Parse).ToObservable();

            var game = new Game(frames);

            Assert.That(game.Score(), Is.EqualTo(31));
        }

        [Test]
        public void StrikeFrameScoresNextTwoDouble()
        {
            var frames = "10,7,3".Split(',').Select(int.Parse).ToObservable();

            var game = new Game(frames);

            Assert.That(game.Score(), Is.EqualTo(30));
        }

        [Test]
        public void TwoStrikeFrameScoresNextTwoDouble()
        {
            var frames = "10,10,7,3".Split(',').Select(int.Parse).ToObservable();

            var game = new Game(frames);

            Assert.That(game.Score(), Is.EqualTo(57));
        }

        [Test]
        public void PerfectGame()
        {
            var frames = "10,10,10,10,10,10,10,10,10,10,10,10".Split(',').Select(int.Parse).ToObservable();

            var game = new Game(frames);

            Assert.That(game.Score(), Is.EqualTo(300));
        }
    }

    public class Game
    {
        private readonly IObservable<int> _score;
        private int _frameScore;
        private bool _firstBall = true;
        private int _nextBallCoefficient = 1;
        private int _ballAfterNextCoefficient = 1;
        private int _frameCount;

        public Game(IObservable<int> frames)
        {
            _score = frames.Scan(0, Accumulator);
        }

        private int Accumulator(int acc, int current)
        {
            _frameScore += current;
            
            var score = acc + (_nextBallCoefficient * current);
            _nextBallCoefficient = _ballAfterNextCoefficient;
            _ballAfterNextCoefficient = 1;

            var isStrike = IsStrike();
            var isSpare = IsSpare();
            var isFrame = IsFrame();

            if (isStrike)
            {
                _nextBallCoefficient++;
                _ballAfterNextCoefficient++;
                _firstBall = true;
                _frameScore = 0;
                _frameCount++;
            }
            else if (isSpare)
            {
                _nextBallCoefficient++;
                _frameScore = 0;
                _firstBall = true;
                _frameCount++;
            }
            else if (isFrame)
            {
                _frameScore = 0;
                _firstBall = true;
                _frameCount++;
            }
            else
            {
                _firstBall = false;
            }
            return score;
        }

        private bool IsFrame()
        {
            return !_firstBall && !IsLastFrame();
        }

        private bool IsSpare()
        {
            return !_firstBall && (_frameScore == 10) && !IsLastFrame();
        }

        private bool IsStrike()
        {
            return _firstBall && (_frameScore == 10) && !IsLastFrame();
        }

        private bool IsLastFrame()
        {
            return (_frameCount >= 9);
        }

        public int Score()
        {
            return _score.Last();
        }
    }
}
