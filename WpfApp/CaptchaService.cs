using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp
{
    public class CaptchaService
    {
        private string _currentCaptcha;
        private static readonly Random _random = new Random();
        private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        private const int CaptchaLength = 5;

        public int FailedAttempts { get; set; }
        public bool IsCaptchaRequired => FailedAttempts >= 3;

        public string GenerateCaptcha()
        {
            var result = new char[CaptchaLength];
            for (int i = 0; i < CaptchaLength; i++)
            {
                result[i] = Chars[_random.Next(Chars.Length)];
            }
            _currentCaptcha = new string(result);
            return _currentCaptcha;
        }

        public bool ValidateCaptcha(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(_currentCaptcha))
                return false;
            return string.Equals(input, _currentCaptcha, StringComparison.Ordinal);
        }

        public string GetCurrentCaptcha()
        {
            return _currentCaptcha;
        }

        public void ResetFailedAttempts()
        {
            FailedAttempts = 0;
        }

        public void IncrementFailedAttempts()
        {
            FailedAttempts++;
        }

        public Canvas RenderCaptchaImage(int width, int height)
        {
            var canvas = new Canvas
            {
                Width = width,
                Height = height,
                Background = Brushes.White
            };

            // Шум: случайные линии
            for (int i = 0; i < 8; i++)
            {
                var line = new Line
                {
                    X1 = _random.Next(width),
                    Y1 = _random.Next(height),
                    X2 = _random.Next(width),
                    Y2 = _random.Next(height),
                    Stroke = new SolidColorBrush(Color.FromRgb(
                        (byte)_random.Next(150, 220),
                        (byte)_random.Next(150, 220),
                        (byte)_random.Next(150, 220))),
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
            }

            // Шум: случайные точки
            for (int i = 0; i < 30; i++)
            {
                var dot = new Ellipse
                {
                    Width = 3,
                    Height = 3,
                    Fill = new SolidColorBrush(Color.FromRgb(
                        (byte)_random.Next(100, 200),
                        (byte)_random.Next(100, 200),
                        (byte)_random.Next(100, 200)))
                };
                Canvas.SetLeft(dot, _random.Next(width));
                Canvas.SetTop(dot, _random.Next(height));
                canvas.Children.Add(dot);
            }

            // Текст капчи
            if (_currentCaptcha != null)
            {
                for (int i = 0; i < _currentCaptcha.Length; i++)
                {
                    var textBlock = new TextBlock
                    {
                        Text = _currentCaptcha[i].ToString(),
                        FontSize = _random.Next(18, 26),
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(
                            (byte)_random.Next(0, 100),
                            (byte)_random.Next(0, 100),
                            (byte)_random.Next(0, 100))),
                        RenderTransform = new RotateTransform(_random.Next(-20, 20))
                    };
                    Canvas.SetLeft(textBlock, 15 + i * (width - 30) / CaptchaLength);
                    Canvas.SetTop(textBlock, _random.Next(5, height - 30));
                    canvas.Children.Add(textBlock);
                }
            }

            return canvas;
        }
    }
}
