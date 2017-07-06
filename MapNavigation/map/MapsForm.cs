using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using FirebirdSql.Data.FirebirdClient;



namespace MapNavigation
{
    public partial class MapsForm : Form
    {
        public MapsForm()
        {
            InitializeComponent();
        }
        FbConnection fb;


        //Переменная отвечающая за состояние нажатия 
        //левой клавиши мыши.
        private bool isLeftButtonDown = false;

        //Таймер для вывода
        private Timer blinkTimer = new Timer();

        //Переменная нового класса,
        //для замены стандартного маркера.
        private MapNavigation.GMI currentMarker;

        //Список маркеров.
        private GMap.NET.WindowsForms.GMapOverlay markersOverlay;


        private void Load_MForm(object sender, EventArgs e)
        {
            //Настройки для компонента GMap.
            gMapControl1.Bearing = 0;

            //CanDragMap - Если параметр установлен в True,
            //пользователь может перетаскивать карту 
            ///с помощью правой кнопки мыши. 
            gMapControl1.CanDragMap = true;

            //Указываем, что перетаскивание карты осуществляется 
            //с использованием левой клавишей мыши.
            //По умолчанию - правая.
            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.GrayScaleMode = true;

            //MarkersEnabled - Если параметр установлен в True,
            //любые маркеры, заданные вручную будет показаны.
            //Если нет, они не появятся.
            gMapControl1.MarkersEnabled = true;

            //Указываем значение максимального приближения.
            gMapControl1.MaxZoom = 18;

            //Указываем значение минимального приближения.
            gMapControl1.MinZoom = 2;

            //Устанавливаем центр приближения/удаления
            //курсор мыши.
            gMapControl1.MouseWheelZoomType =
                GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

            //Отказываемся от негативного режима.
            gMapControl1.NegativeMode = false;

            //Разрешаем полигоны.
            gMapControl1.PolygonsEnabled = true;

            //Разрешаем маршруты
            gMapControl1.RoutesEnabled = true;

            //Скрываем внешнюю сетку карты
            //с заголовками.
            gMapControl1.ShowTileGridLines = false;

            //Указываем, что при загрузке карты будет использоваться 
            //18ти кратной приближение.
            gMapControl1.Zoom = 15;

            //Указываем что все края элемента управления
            //закрепляются у краев содержащего его элемента
            //управления(главной формы), а их размеры изменяются 
            //соответствующим образом.
            gMapControl1.Dock = DockStyle.None;

            //Указываем что будем использовать карты Google.
            gMapControl1.MapProvider =
                GMap.NET.MapProviders.GMapProviders.YandexMap;
            GMap.NET.GMaps.Instance.Mode =
                GMap.NET.AccessMode.ServerOnly;

            //Если вы используете интернет через прокси сервер,
            //указываем свои учетные данные.
            GMap.NET.MapProviders.GMapProvider.WebProxy =
                System.Net.WebRequest.GetSystemWebProxy();
            GMap.NET.MapProviders.GMapProvider.WebProxy.Credentials =
                System.Net.CredentialCache.DefaultCredentials;

            //Устанавливаем координаты центра карты для загрузки.
            gMapControl1.Position = new GMap.NET.PointLatLng(55.75393, 37.620795);

            //Создаем новый список маркеров, с указанием компонента 
            //в котором они будут использоваться и названием списка.
            markersOverlay = new GMap.NET.WindowsForms.GMapOverlay("metka");

            //Устанавливаем свои методы на события.
            //   gMapControl1.OnMapZoomChanged   += new MapZoomChanged(mapControl_OnMapZoomChanged);
            //  gMapControl1.MouseClick              += new MouseEventHandler(mapControl_MouseClick);
            gMapControl1.MouseDown += new MouseEventHandler(Кнопка_Нажата);
            gMapControl1.MouseUp += new MouseEventHandler(Кнопка_отпущена);
            gMapControl1.MouseMove += new MouseEventHandler(Перетаскивание);
            gMapControl1.OnMarkerClick += new MarkerClick(клик_по_маркеру);
            gMapControl1.OnMarkerEnter += new MarkerEnter(выбор_макркера);
            gMapControl1.OnMarkerLeave += new MarkerLeave(mapControl_OnMarkerLeave);

            //Добавляем в элемент управления карты
            //список маркеров.
            gMapControl1.Overlays.Add(markersOverlay); ;


            //формируем connection string для последующего соединения с нашей базой данных
            FbConnection fb_con = new FbConnection(BDConnect.ConnectionStr);
            fb_con.Open();
            string queryBrand = "select NAME_ from ROUTS";
            FbCommand com = new FbCommand(queryBrand, fb_con);
            FbDataReader dr = com.ExecuteReader();
            while (dr.Read())
            {
                 comboBox1.Items.Add(dr.GetString(5));
             //   Console.WriteLine(dr.GetString(0));
            }
            dr.Close();
            fb_con.Close();
            Console.ReadLine();
           
           
        }

        private void SetColumName(DataTable tableBrand)
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Перетаскивание(object sender, MouseEventArgs e)
        {
            //Проверка, что нажата левая клавиша мыши.
            if (e.Button == System.Windows.Forms.MouseButtons.Left && isLeftButtonDown)
            {
                if (currentMarker != null)
                {
                    PointLatLng point =
                        gMapControl1.FromLocalToLatLng(e.X, e.Y);
                    //Получение координат маркера.
                    currentMarker.Position = point;
                    //Вывод координат маркера в подсказке.
                    currentMarker.ToolTipText =
                        string.Format("{0},{1}", point.Lat, point.Lng);
                }
            }
        }

        private void КМ(object sender, MouseEventArgs e)
        {

            //Выполняем проверку, какая клавиша мыши была нажата,
            //если правая, то выполняем установку маркера.
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //Если надо установить только один маркер,
                //то выполняем очистку списка маркеров
                markersOverlay.Markers.Clear();
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                //Инициализируем новую переменную изображения и
                //загружаем в нее изображение маркера,
                //лежащее возле исполняемого файла
                Bitmap bitmap =
                    Bitmap.FromFile(Application.StartupPath + @"\123.png") as Bitmap;

                //Инициализируем новый маркер с использованием 
                //созданного нами маркера.
                GMapMarker marker = new GMI(point, bitmap);
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                //В качестве подсказки к маркеру устанавливаем 
                //координаты где он устанавливается.
                //Данные о местоположении маркера, вы можете вывести в любой компонент
                //который вам нужен.
                //например:
                //textBo1.Text = point.Lat;
                //textBo2.Text = point.Lng;
                marker.ToolTipText = string.Format("{0},{1}", point.Lat, point.Lng);

                //Добавляем маркер в список маркеров.
                markersOverlay.Markers.Add(marker);
            }
        }

        private void Кнопка_отпущена(object sender, MouseEventArgs e)
        {
            //Выполняем проверку, какая клавиша мыши была отпущена,
            //если левая, то устанавливаем переменной значение false.
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isLeftButtonDown = false;
            }
        }

        private void Кнопка_Нажата(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isLeftButtonDown = true;
            }
        }
        void клик_по_маркеру(GMapMarker item, MouseEventArgs e)
        {
        }

        void выбор_макркера(GMapMarker item)
        {
            if (item is GMI)
            {
                currentMarker = item as GMI;
                currentMarker.Pen = new Pen(Brushes.Red, 1);
            }
        }
        void mapControl_OnMarkerLeave(GMapMarker item)
        {
            if (item is GMI)
            {
                currentMarker = null;
                GMI m = item as GMI;
                m.Pen.Dispose();
                m.Pen = null;
            }
        }

        private void подключитьБдToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

            }

        private void отчетУГДНToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Params Pr = new Params();
         //   Pr.Size=new Size(165,237);
            Pr.Location=new Point(12,27);
            this.Controls.Add(Pr);
        }


        
    }
}
       