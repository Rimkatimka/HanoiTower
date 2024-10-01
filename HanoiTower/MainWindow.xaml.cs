using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HanoiTower
{
    public partial class MainWindow : Window
    {
        private Stack<Rectangle> sourceTower = new Stack<Rectangle>();
        private Stack<Rectangle> targetTower = new Stack<Rectangle>();
        private Stack<Rectangle> auxiliaryTower = new Stack<Rectangle>();

        private Rectangle selectedDisk = null;
        private Point mouseOffset;

        private int moveCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Описулька кнопки
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int diskCount;
            if (int.TryParse(diskCountTextBox.Text, out diskCount) && diskCount > 0)
            {
                InitializeTowers(diskCount);
                moveCounter = 0;
                moveCounterTextBlock.Text = moveCounter.ToString();
            }
            else
            {
                MessageBox.Show("Введите корректное количество дисков.");
            }
        }

        private void InitializeTowers(int diskCount)
        {
            sourceTower.Clear();
            targetTower.Clear();
            auxiliaryTower.Clear();

            canvas.Children.Clear();

            DrawTowers(); // Рисуем кнопки

            for (int i = diskCount; i >= 1; i--)
            {
                Rectangle disk = new Rectangle
                {
                    Width = i * 20 + 50,
                    Height = 20,
                    Fill = Brushes.Blue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Tag = i
                };


                Canvas.SetLeft(disk, 100 - disk.Width / 2);
                Canvas.SetTop(disk, 300 - (diskCount - i) * 20);

                disk.MouseDown += Disk_MouseDown;
                disk.MouseMove += Disk_MouseMove;
                disk.MouseUp += Disk_MouseUp;

                canvas.Children.Add(disk);
                sourceTower.Push(disk);
            }
        }

        // Рисуем палки
        private void DrawTowers()
        {
            double towerWidth = 10;
            double towerHeight = 200;

            // Левая палка
            Line leftTower = new Line { X1 = 100, Y1 = 300 - towerHeight, X2 = 100, Y2 = 300, Stroke = Brushes.Black, StrokeThickness = towerWidth };
            canvas.Children.Add(leftTower);

            // Средняя палка
            Line middleTower = new Line { X1 = 250, Y1 = 300 - towerHeight, X2 = 250, Y2 = 300, Stroke = Brushes.Black, StrokeThickness = towerWidth };
            canvas.Children.Add(middleTower);

            // Правая палка
            Line rightTower = new Line { X1 = 400, Y1 = 300 - towerHeight, X2 = 400, Y2 = 300, Stroke = Brushes.Black, StrokeThickness = towerWidth };
            canvas.Children.Add(rightTower);
        }

        private void Disk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle disk = sender as Rectangle;


            if (IsTopDisk(disk))
            {
                selectedDisk = disk;
                if (selectedDisk != null)
                {
                    mouseOffset = e.GetPosition(canvas);
                    mouseOffset.X -= Canvas.GetLeft(selectedDisk);
                    mouseOffset.Y -= Canvas.GetTop(selectedDisk);
                    Canvas.SetZIndex(selectedDisk, 100);
                }
            }
        }

        private void Disk_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedDisk != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePosition = e.GetPosition(canvas);

                Canvas.SetLeft(selectedDisk, mousePosition.X - mouseOffset.X);
                Canvas.SetTop(selectedDisk, mousePosition.Y - mouseOffset.Y);
            }
        }

        private void Disk_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedDisk != null)
            {
                double diskX = Canvas.GetLeft(selectedDisk);

                Stack<Rectangle> targetStack = null;
                if (diskX < 150) targetStack = sourceTower;
                else if (diskX > 350) targetStack = targetTower;
                else targetStack = auxiliaryTower;

                if (CanPlaceDisk(targetStack))
                {
                    PlaceDiskOnTower(targetStack);
                    moveCounter++;
                    moveCounterTextBlock.Text = moveCounter.ToString();

                    CheckForWin();
                }
                else
                {

                    ReturnDiskToOriginalPosition();
                }

                selectedDisk = null;
            }
        }

        private bool CanPlaceDisk(Stack<Rectangle> targetStack)
        {
            if (targetStack.Count == 0) return true;

            Rectangle topDisk = targetStack.Peek();
            return (int)selectedDisk.Tag < (int)topDisk.Tag;
        }

        private void PlaceDiskOnTower(Stack<Rectangle> targetStack)
        {
            Stack<Rectangle> previousStack = FindDiskStack(selectedDisk);

            if (previousStack != null)
            {
                previousStack.Pop();
            }

            targetStack.Push(selectedDisk);


            double xPosition = 0;
            if (targetStack == sourceTower) xPosition = 100;
            else if (targetStack == auxiliaryTower) xPosition = 250;
            else if (targetStack == targetTower) xPosition = 400;

            double yPosition = 300 - targetStack.Count * 20;
            Canvas.SetLeft(selectedDisk, xPosition - selectedDisk.Width / 2);
            Canvas.SetTop(selectedDisk, yPosition);
        }


        private void ReturnDiskToOriginalPosition()
        {
            Stack<Rectangle> originalStack = FindDiskStack(selectedDisk);

            if (originalStack != null)
            {
                double xPosition = 0;
                if (originalStack == sourceTower) xPosition = 100;
                else if (originalStack == auxiliaryTower) xPosition = 250;
                else if (originalStack == targetTower) xPosition = 400;

                double yPosition = 300 - originalStack.Count * 20;
                Canvas.SetLeft(selectedDisk, xPosition - selectedDisk.Width / 2);
                Canvas.SetTop(selectedDisk, yPosition);
            }
        }


        private bool IsTopDisk(Rectangle disk)
        {
            Stack<Rectangle> towerStack = FindDiskStack(disk);
            return towerStack != null && towerStack.Peek() == disk;
        }


        private Stack<Rectangle> FindDiskStack(Rectangle disk)
        {
            if (sourceTower.Contains(disk)) return sourceTower;
            if (auxiliaryTower.Contains(disk)) return auxiliaryTower;
            if (targetTower.Contains(disk)) return targetTower;
            return null;
        }


        private void CheckForWin()
        {
            if (targetTower.Count == int.Parse(diskCountTextBox.Text))
            {
                MessageBox.Show($"Вы выиграли за {moveCounter} ходов.");
            }
        }
    }
}
