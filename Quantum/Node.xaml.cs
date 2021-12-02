using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Quantum
{
    /// <summary>
    /// Логика взаимодействия для Node.xaml
    /// </summary>
    public partial class Node : System.Windows.Controls.UserControl
    {
        #region Внутренние переменные
        private Node parent;
        private ObservableCollection<Node> children = new ObservableCollection<Node>();
        private bool hasparent = true;
        private bool haschildren = true;
        private Brush parentbrush = new SolidColorBrush(Colors.Red);
        private Brush linebrush = new SolidColorBrush(Colors.Green);
        private Point StartDrag;
        private string NameText;
        private string InfoText;
        private NodeType type = NodeType.Optimization;
        private long id = 0;
        private SQLiteDataBase DB;
        private Job job;
        private bool selected = false;
        private bool Moving = false;
        private bool SingleSelection = false;
        private List<Line> ChildrenLines = new List<Line>();
        private bool? parallelrun = null;

        #region Цвета кистей
        private static byte MainColor1 = 0xE6;
        private static byte MainColor2 = 0x83;
        private static byte MainColor3 = 0x4D;
        private static byte MainColor4 = 0x2B;
        private static byte SubColor1 = 0x34;
        private static byte SubColor2 = 0x1D;
        private static byte SubColor3 = 0x11;
        private static byte SubColor4 = 0x0A;
        private static byte SelectedColor1 = 0xFF;
        private static byte SelectedColor2 = 0xA3;
        private static byte SelectedColor3 = 0x6D;
        private static byte SelectedColor4 = 0x4B;
        #endregion
        #endregion

        #region Кисти
        private static Brush InputBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(MainColor1,MainColor1,0x00), 0.0),
            new GradientStop(Color.FromRgb(MainColor2,MainColor2,0x00), 0.063),
            new GradientStop(Color.FromRgb(MainColor3,MainColor3,0x00), 0.937),
            new GradientStop(Color.FromRgb(MainColor4,MainColor4,0x00), 1.0)
        }), 90);
        private static Brush OptimizationBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() 
        { 
            new GradientStop(Color.FromRgb(SubColor1,MainColor1,0x00), 0.0),
            new GradientStop(Color.FromRgb(SubColor2,MainColor2,0x00), 0.063),
            new GradientStop(Color.FromRgb(SubColor3,MainColor3,0x00), 0.937),
            new GradientStop(Color.FromRgb(SubColor4,MainColor4,0x00), 1.0)
        }), 90);
        private static Brush StopBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(MainColor1,SubColor1,0x00), 0.0),
            new GradientStop(Color.FromRgb(MainColor2,SubColor2,0x00), 0.063),
            new GradientStop(Color.FromRgb(MainColor3,SubColor3,0x00), 0.937),
            new GradientStop(Color.FromRgb(MainColor4,SubColor4,0x00), 1.0)
        }), 90);
        private static Brush CommentBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(0x00,0x00,MainColor1), 0.0),
            new GradientStop(Color.FromRgb(0x00,0x00,MainColor2), 0.063),
            new GradientStop(Color.FromRgb(0x00,0x00,MainColor3), 0.937),
            new GradientStop(Color.FromRgb(0x00,0x00,MainColor4), 1.0)
        }), 90);
        private static Brush RunBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(MainColor1,0x00,MainColor1), 0.0),
            new GradientStop(Color.FromRgb(MainColor2,0x00,MainColor2), 0.063),
            new GradientStop(Color.FromRgb(MainColor3,0x00,MainColor3), 0.937),
            new GradientStop(Color.FromRgb(MainColor4,0x00,MainColor4), 1.0)
        }), 90);
        private static Brush OptimizationSelectedBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(SubColor1,SelectedColor1,0x00), 0.0),
            new GradientStop(Color.FromRgb(SubColor2,SelectedColor2,0x00), 0.063),
            new GradientStop(Color.FromRgb(SubColor3,SelectedColor3,0x00), 0.937),
            new GradientStop(Color.FromRgb(SubColor4,SelectedColor4,0x00), 1.0)
        }), 90);
        private static Brush StopSelectedBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(SelectedColor1,SubColor1,0x00), 0.0),
            new GradientStop(Color.FromRgb(SelectedColor2,SubColor2,0x00), 0.063),
            new GradientStop(Color.FromRgb(SelectedColor3,SubColor3,0x00), 0.937),
            new GradientStop(Color.FromRgb(SelectedColor4,SubColor4,0x00), 1.0)
        }), 90);
        private static Brush CommentSelectedBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(0x00,0x00,SelectedColor1), 0.0),
            new GradientStop(Color.FromRgb(0x00,0x00,SelectedColor2), 0.063),
            new GradientStop(Color.FromRgb(0x00,0x00,SelectedColor3), 0.937),
            new GradientStop(Color.FromRgb(0x00,0x00,SelectedColor4), 1.0)
        }), 90);
        private static Brush RunSelectedBrush = new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>()
        {
            new GradientStop(Color.FromRgb(SelectedColor1,0x00,SelectedColor1), 0.0),
            new GradientStop(Color.FromRgb(SelectedColor2,0x00,SelectedColor2), 0.063),
            new GradientStop(Color.FromRgb(SelectedColor3,0x00,SelectedColor3), 0.937),
            new GradientStop(Color.FromRgb(SelectedColor4,0x00,SelectedColor4), 1.0)
        }), 90);

        private static Brush SelectedBorderBrush = new SolidColorBrush(Colors.DarkBlue);

        private static Dictionary<NodeType, Brush> MainBrushes = new Dictionary<NodeType, Brush>()
        { {NodeType.Optimization, OptimizationBrush}, {NodeType.End, StopBrush }, {NodeType.Comment, CommentBrush }, {NodeType.Run, RunBrush } , { NodeType.Input, InputBrush} };
        private static Dictionary<NodeType, Brush> SelectedBrushes = new Dictionary<NodeType, Brush>()
        { {NodeType.Optimization, OptimizationSelectedBrush}, {NodeType.End, StopSelectedBrush }, {NodeType.Comment, CommentSelectedBrush }, {NodeType.Run, RunSelectedBrush } , { NodeType.Input, InputBrush} };
        #endregion

        #region Свойства
        /// <summary>
        /// Родительский узел. Может быть только один.
        /// </summary>
        public Node ParentNode
        {
            get => parent;
            set
            {
                if (parent != null)
                {
                    parent.ChildrenNodes.Remove(this);
                    ((NodePanel)Parent).Children.Remove(ParentLine);
                    parent.ChildrenLines.Remove(ParentLine);
                }
                parent = value;
                ParentRect.Fill = parent == null ? null : parentbrush;
                if (parent == null) ParentLine = null;
                else
                {
                    ParentLine = new Line() { Stroke = linebrush, StrokeThickness = 1.5 };
                    ((Grid)this.Parent).Children.Add(ParentLine);
                    parent.ChildrenLines.Add(ParentLine);
                }

                parent?.ChildrenNodes.Add(this);

                if (parent == null) 
                    DB.Execute($"UPDATE `nodes` SET `parent`= 0 WHERE `id`={ID}");
                else
                    DB.Execute($"UPDATE `nodes` SET `parent`= {parent.ID} WHERE `id`={ID}");

                ParentChanged?.Invoke(this, new EventArgs());

                RePaint();
            }
        }

        /// <summary>
        /// Линия, соединяющая родителя и потомка
        /// </summary>
        public Line ParentLine;

        /// <summary>
        /// Список узлов-потомков
        /// </summary>
        public ObservableCollection<Node> ChildrenNodes
        {
            get => children;
            set
            {
                children = value;
                children.CollectionChanged -= ChildrenListChanged;
                children.CollectionChanged += ChildrenListChanged;
                ChildrenChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Положение стыковки для родительского узла
        /// </summary>
        public Point ParentPosition {
            get
            {
                return new Point(Margin.Left + ParentRect.ActualWidth / 2, Margin.Top + ActualHeight / 2);
            }
        }

        /// <summary>
        /// Положение стыковки для узлов-потомков
        /// </summary>
        public Point ChildrenPosition
        {
            get
            {
                return new Point(Margin.Left + ActualWidth - ParentRect.ActualWidth / 2, Margin.Top + ActualHeight / 2);
            }
        }

        /// <summary>
        /// Должен ли у узла быть родитель
        /// </summary>
        public bool HasParent
        {
            get => hasparent;
            protected set
            {
                hasparent = value;
                ParentRect.Visibility = hasparent
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Должны ли у узла быть потомки
        /// </summary>
        public bool HasChildren
        {
            get => haschildren;
            protected set
            {
                haschildren = value;
                ChildrenRect.Visibility = haschildren
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Заголовок узла.
        /// </summary>
        public string Title
        {
            get => NameText;
            set
            {
                NameText = value;
                NameLabel.Content = Type == NodeType.Run ? "▶ " + NameText : NameText;
                this.Height = RealHeight();
            }
        }

        /// <summary>
        /// Подробная информация об узле
        /// </summary>
        public string Info
        {
            get => InfoText;
            set
            {
                InfoText = value;
                InfoBlock.Text = InfoText;
                InfoBlock.Visibility = InfoText == "" ? Visibility.Collapsed : Visibility.Visible;
                this.Height = RealHeight();
            }
        }

        /// <summary>
        /// Флаг перемещения узла
        /// </summary>
        public bool Drag { get; private set; }

        /// <summary>
        /// Тип узла. Может принимать значения «Ввод», «Оптимизация», «Параметрический» и «Комментарий»
        /// </summary>
        public NodeType Type
        {
            get => type;
            set
            {
                type = value;
                switch(type)
                {
                    case NodeType.Input:
                        SetNode(false, true, 10, 80);
                        ParentNode = null;
                        break;
                    case NodeType.Optimization:
                        SetNode(true, true, 3, 150);
                        break;
                    case NodeType.End:
                        SetNode(true, false, 3, 150);
                        foreach (Node N in ChildrenNodes.ToList())
                            N.ParentNode = null;
                        break;
                    case NodeType.Comment:
                        SetNode(true, false, 3, 150);
                        foreach (Node N in ChildrenNodes.ToList())
                            N.ParentNode = null;
                        break;
                    case NodeType.Run:
                        SetNode(true, true, 10, 150);
                        break;
                }

                FillRect.Fill = MainBrushes[Type];
            }
        }

        /// <summary>
        /// Номер узла в БД
        /// </summary>
        public long ID
        {
            get => id;
            private set
            {
                // Заменить номер можно только с нуля.
                if (id > 0) return;
                if (value <= 0) return;
                id = value;
            }
        }

        /// <summary>
        /// Ссылка на номер задания в БД, с которым ассоциирован узел
        /// </summary>
        public Job NodeJob
        {
            get => job;
            set
            {
                if (Type == NodeType.Optimization || Type == NodeType.End)
                {
                    Type = value.Task == 1 ? NodeType.End : NodeType.Optimization;

                    job = value;
                    if (job == null) return;
                    _ = DB.Execute($"UPDATE `nodes` SET `job`= {job.ID} WHERE `id`={ID}");

                    Title = job.Name;
                    Info = job.ToString();
                    this.Height = RealHeight();
                }
                else
                    job = null;
            }
        }

        /// <summary>
        /// Флаг выделения узла
        /// </summary>
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                FillRect.Fill = selected ? SelectedBrushes[Type] : MainBrushes[Type];
                FillRect.Stroke = selected ? SelectedBorderBrush : null;
            }
        }

        /// <summary>
        /// Событие, возникающее при изменении родителя
        /// </summary>
        public event EventHandler ParentChanged;

        /// <summary>
        /// Событие, возникающее при любом изменении списка потомков
        /// </summary>
        public event EventHandler ChildrenChanged;

        /// <summary>
        /// Многоядерный запуск программ. Только для типа Run
        /// </summary>
        public bool? ParallelRun
        {
            get => parallelrun;
            set
            {
                if (Type == NodeType.Run)
                {
                    parallelrun = value == null ? false : value;
                    int Par = parallelrun == true ? 1 : 0;
                    DB.Execute($"UPDATE `nodes` SET `job`={Par} WHERE `id`={ID};");
                }
                else
                    parallelrun = null;
            }
        }
        #endregion

        #region Конструкторы
        public Node()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка одиночного узла из БД
        /// </summary>
        /// <param name="DB">База данных SQLite</param>
        /// <param name="ID">Номер узла</param>
        /// <returns></returns>
        public static Node LoadFromDB(SQLiteDataBase DB, long ID)
        {
            using (DataTable dt = DB.ReadTable($"SELECT * FROM `nodes` WHERE `id`={ID};"))
            {
                if (dt.Rows.Count == 0) return null;

                DataRow dr = dt.Rows[0];

                NodeType NT = (NodeType)Enum.ToObject(typeof(NodeType), dr.Field<long>("type"));
                string name = dr.Field<string>("name");

                Node NN = new Node()
                {
                    DB = DB,
                    ID = ID,
                    Type = NT,
                    Title = name,
                    Info = dr.Field<string>("comment"), 
                    Margin = new Thickness(dr.Field<double>("pos_x"), dr.Field<double>("pos_y"), 0, 0)
                };

                return NN;
            }
        }
        #endregion

        #region Методы

        /// <summary>
        /// Перерисовка узла
        /// </summary>
        public void RePaint()
        {
            if (ParentNode != null)
            {
                Point ParentPoint = ParentNode.ChildrenPosition;
                Point MyPoint = ParentPosition;
                ParentLine.X1 = ParentPoint.X;
                ParentLine.Y1 = ParentPoint.Y;
                ParentLine.X2 = MyPoint.X;
                ParentLine.Y2 = MyPoint.Y;
            }

            Point MyChildrenPoint = ChildrenPosition;
            for (int i = 0; i < ChildrenNodes.Count; i++)
            {
                Point ChildrenPoint = ChildrenNodes[i].ParentPosition;
                ChildrenLines[i].X1 = MyChildrenPoint.X;
                ChildrenLines[i].Y1 = MyChildrenPoint.Y;
                ChildrenLines[i].X2 = ChildrenPoint.X;
                ChildrenLines[i].Y2 = ChildrenPoint.Y;
            }
        }

        /// <summary>
        /// Прекращение перетаскивания объекта
        /// </summary>
        public void StopDrag()
        {
            if (ID == 0) Save();
            else DB.Execute($"UPDATE `nodes` SET `pos_x`={Margin.Left}, `pos_y`={Margin.Top} WHERE `id`={ID}");
            Drag = false;
        }

        /// <summary>
        /// Получить смещение мыши относительно старого положения
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public Point MoveDelta(Point Position)
        {
            Point MD = new Point(Position.X - StartDrag.X, Position.Y - StartDrag.Y);
            StartDrag = Position;
            return MD;
        }

        /// <summary> 
        /// Перемещение узла
        /// </summary>
        /// <param name="CurrentPosition">Текущее положение курсора</param>
        public void MoveTo(Point CurrentPosition)
        {
            Point Delta = MoveDelta(CurrentPosition);
            Move(Delta);
            RePaint();
        }

        /// <summary>
        /// Перемещение узла на вектор изменения Delta
        /// </summary>
        /// <param name="Delta">Вектор перемещения</param>
        public void Move(Point Delta)
        {
            if (Drag) Moving = true;
            
            double X = Margin.Left + Delta.X;
            if (X < 0) X = 0;
            if (X > ((FrameworkElement)Parent).ActualWidth - ActualWidth) X = ((FrameworkElement)Parent).ActualWidth - ActualWidth;
            double Y = Margin.Top + Delta.Y;
            if (Y < 0) Y = 0;
            if (Y > ((FrameworkElement)Parent).ActualHeight - ActualHeight) Y = ((FrameworkElement)Parent).ActualHeight - ActualHeight;
            Margin = new Thickness(X, Y, 0, 0);
            RePaint();
        }

        /// <summary>
        /// Определение высоты узла
        /// </summary>
        /// <returns></returns>
        private double RealHeight()
        {
            double Res = Data.Margin.Top + Data.Margin.Bottom +
                NameLabel.ActualHeight + NameLabel.Margin.Top + NameLabel.Margin.Bottom +
                InfoBlock.ActualHeight + InfoLabel.Padding.Top + InfoLabel.Padding.Bottom;
            return Res;
        }

        /// <summary>
        /// Сохранение узла в известной базе данных
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            int NType = (int)Type;
            string Comment = InfoText;
            long JobID = 0;
            long Par = ParentNode == null ? 0 : ParentNode.ID;
            if (ID == 0)
            {
                bool OK = DB.Execute($"INSERT INTO `nodes` (`type`, `name`, `parent`, `comment`, `job`, `pos_x`, `pos_y`)" +
                    $"VALUES ({NType}, '{NameText}', {Par}, '{Comment}', {JobID}, {Margin.Left}, {Margin.Top});");
                ID = DB.LastID;
                return OK;
            }

            return DB.Execute($"UPDATE `nodes` SET `type`={NType}, `name`='{NameText}', `parent`={Par}, " +
                $"`comment`='{Comment}', `job`={JobID}, `pos_x`={Margin.Left}, `pos_y`={Margin.Top} WHERE `id`={ID}");
        }

        /// <summary>
        /// Сохранение узла в базе данных
        /// </summary>
        /// <param name="db">База данных SQLite</param>
        /// <returns></returns>
        public bool Save(SQLiteDataBase db)
        {
            DB = db;
            return Save();
        }

        /// <summary>
        /// Загрузка потомков узла
        /// </summary>
        public void LoadChildren()
        {
            using (DataTable dt = DB.ReadTable($"SELECT * FROM `nodes` WHERE `parent`={ID};"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Node NN = new Node()
                    {
                        DB = DB,
                        ID = dr.Field<long>("id"),
                        Type = (NodeType)dr.Field<long>("type"),
                        Title = dr.Field<string>("name"),
                        Info = dr.Field<string>("comment"),
                        Margin = new Thickness(dr.Field<double>("pos_x"), dr.Field<double>("pos_y"), 0, 0)
                    };

                    switch (NN.Type)
                    {
                        case NodeType.Optimization:
                        case NodeType.End:
                            NN.NodeJob = Job.Load(DB, dr.Field<long>("job"));
                            break;
                        case NodeType.Run:
                            NN.ParallelRun = dr.Field<long>("job") == 1;
                            break;
                    }


                    ((NodePanel)Parent).Children.Add(NN);
                    NN.ParentNode = this;
                    NN.LoadChildren();
                }
            }
        }


        /// <summary>
        /// Настройка ноды под тип
        /// </summary>
        /// <param name="hasParent">Наличие родителя</param>
        /// <param name="hasChildren">Наличие потомка</param>
        /// <param name="Radius">Радиус скругления</param>
        /// <param name="fill">Заливка</param>
        /// <param name="width">Ширина</param>
        private void SetNode(bool hasParent, bool hasChildren, double Radius, double width)
        {
            HasParent = hasParent;
            HasChildren = hasChildren;
            FillRect.RadiusX = Radius;
            FillRect.RadiusY = Radius;
            Width = width;
        }

        /// <summary>
        /// Удаляет все ссылки и запись в БД
        /// </summary>
        public void Delete()
        {
            ParentNode = null;
            foreach (Node N in ChildrenNodes.ToArray())
                N.ParentNode = null;
            DB.Execute($"DELETE FROM `nodes` WHERE `id`={ID}");
            if (NodeJob != null)
                DB.Execute($"DELETE FROM `jobs` WHERE `id`={NodeJob.ID}");
        }

        /// <summary>
        /// Обработка входящего сигнала
        /// </summary>
        /// <param name="sygnal"></param>
        /// <returns></returns>
        public Sygnal SendSygnal(Sygnal sygnal = null)
        {
            if (sygnal == null) Console.WriteLine("Создание нового сигнала");
            Sygnal CurrentSygnal = sygnal == null ? new Sygnal(this) : sygnal;
            if (Type == NodeType.Run)
            {
                Console.WriteLine("Создание нового сигнала узлом запуска");
                Sygnal NewSygnal = new Sygnal(ParentNode, Title) { Parallel = ParallelRun == true, ParentParallel = CurrentSygnal.Parallel };
                foreach (Node N in ChildrenNodes.Where(x => x.Type != NodeType.Comment))
                    NewSygnal = N.SendSygnal(NewSygnal);
                ((NodePanel)Parent).Sygnals.Insert(0, NewSygnal);

                NewSygnal.MakeRun("D:\\Temp\\Project", "Test/1/01", (SQLiteConfig)DB);

                return CurrentSygnal;
            }

            if (Type != NodeType.Input) 
                Console.WriteLine($"Добавление задания {NodeJob.Name} в список");
            CurrentSygnal.Jobs.Add(NodeJob);
            foreach (Node N in ChildrenNodes.Where(x => x.Type != NodeType.Comment))
                CurrentSygnal = N.SendSygnal(CurrentSygnal);
            return CurrentSygnal;
        }

        /// <summary>
        /// Запуск сигнала из INPUT узла
        /// </summary>
        /// <returns></returns>
        public bool StartSygnal()
        {
            if (Type != NodeType.Input) return false;

            SendSygnal();
            return true;
        }
        #endregion

        #region События
        private void ChildrenRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((NodePanel)Parent).ConnectionParent = this;
        }

        private void ChildrenRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (((NodePanel)Parent).ConnectionParent == this) ((NodePanel)Parent).ConnectionParent = null;

            if (((NodePanel)Parent).ConnectionChild != null)
            {
                ((NodePanel)Parent).ConnectionChild.ParentNode = this;
                ((NodePanel)Parent).ConnectionChild = null;
            }
        }

        private void ParentRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (((NodePanel)Parent).ConnectionChild == this) ((NodePanel)Parent).ConnectionChild = null;

            if (((NodePanel)Parent).ConnectionParent != null)
            {
                ParentNode = ((NodePanel)Parent).ConnectionParent;
                ((NodePanel)Parent).ConnectionParent = null;
            }
        }

        private void ParentRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((NodePanel)Parent).ConnectionChild = this;
        }

        private void Rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Height = RealHeight();
        }

        /// <summary>
        /// Событие. Изменение списка потомков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildrenListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ChildrenChanged?.Invoke(this, new EventArgs());
        }
        
        /// <summary>
        /// Событие. Кнопка нажата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartDrag = e.GetPosition((IInputElement)Parent);
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    switch (Type)
                    {
                        case NodeType.Run:
                            RunData runData = RunWindow.EditRun(new RunData() { Parallel = ParallelRun == true, Text = Title });
                            if (runData.OK)
                            {
                                ParallelRun = runData.Parallel;
                                Title = runData.Text;
                            }
                            break;
                        case NodeType.Optimization:
                        case NodeType.End:
                            Job newJob = JobEdit.Edit((SQLiteConfig)DB, NodeJob);
                            if (newJob != null) NodeJob = newJob;
                            RePaint();
                            break;
                        case NodeType.Comment:
                            KeyValuePair<string, string> Data = AddParam.Edit("Редактирование комментария", new KeyValuePair<string, string>(Title, Info), "Заголовок", "Содержание");
                            if (Data.Key == null) break;
                            Title = Data.Key;
                            Info = Data.Value;
                            break;
                    }
                    return;
                }

                Drag = true;
                Moving = false;
                SingleSelection = false;

                if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
                {
                    if (!Selected)
                    {
                        Selected = true;
                        Drag = false;
                    }
                    else
                        SingleSelection = true;
                }

                else
                {

                    if (!Selected)
                    {
                        ((NodePanel)Parent).DeselectAll();
                        Selected = true;
                    }
                    else
                        SingleSelection = true;
                }
            }
        }

        /// <summary>
        /// Событие. Кнопка отпущена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2)
            {
                Console.WriteLine("Двойной клик");
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
                if (SingleSelection && !Moving)
                    Selected = false;

            ((NodePanel)Parent).SaveSelected();

            e.Handled = true;
        }

        /// <summary>
        /// Событие. Движение мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        #endregion
    }

    /// <summary>
    /// Определяет тип узла.
    /// </summary>
    public enum NodeType { Input, Optimization, End, Comment, Run }
}
