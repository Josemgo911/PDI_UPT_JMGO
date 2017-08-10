
//Multiple face detection and recognition in real time
//Using EmguCV cross platform .Net wrapper to the Intel OpenCV image processing library for C#.Net
//Writed by Sergio Andrés Guitérrez Rojas
//"Serg3ant" for the delveloper comunity
// Sergiogut1805@hotmail.com
//Regards from Bucaramanga-Colombia ;)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;
using BackgroundWorkerDemo;

namespace MultiFaceRec
{
    public partial class FrmPrincipal : Form
    {
        //Declararation of all variables, vectors and haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels= new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;
        splashscreen splash;
        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void setSplash(splashscreen spl)
        {
            splash = spl;
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            splash.Dispose();
        }

        private void btnCargarEmocion_Click(object sender, EventArgs e)
        {
            
        }

        private void entrenarConFotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnf = new OpenFileDialog();

            if (opnf.ShowDialog() == DialogResult.OK)
            {
                string url = opnf.FileName;
                Image<Gray, byte> gray = new Image<Gray, byte>(url);

                string emocion = Microsoft.VisualBasic.Interaction.InputBox(
               "¡Que emocion se entrenara?",
               "Selección de emoción.",
               "Feliz");

                entrenarBD(gray,emocion);
                MessageBox.Show("La emoción " + emocion + " se a guardado correctamente", "Imagen guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void entrenarBD(Image<Gray, byte> gray,String emocion)
        {
            Image<Gray, byte> gray2 = gray;
            gray2.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            //MessageBox.Show("Se creo la imagen");
            imageBox1.Image = gray2;
            //Trained face counter
            ContTrain = ContTrain + 1;
            /////////////

            MCvAvgComp[][] facesDetected = gray2.DetectHaarCascade(
            face,
            1.2,
            10,
            Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
            new Size(20, 20));

            //Action for each element detected
            foreach (MCvAvgComp f in facesDetected[0])
            {
                //TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                TrainedFace = gray2.Copy(f.rect).Convert<Gray, byte>();
                break;
            }

            //resize face detected image for force to compare the same size with the 
            //test image with cubic interpolation type method
            //TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
            trainingImages.Add(TrainedFace);
            labels.Add(emocion);

            //Show face added in gray scale
            imageBox1.Image = TrainedFace;

            //Write the number of triained faces in a file text for further load
            File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");
            //File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
            //Write the labels of triained faces in a file text for further load
            for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
            {
                trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedFaces/face" + i + ".jpg");
                File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");

            }
            //trainingImages.Clear();
            lbEmocion.Text = "Emoción: " + emocion;

        }

        private void entrenarConDirectorioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog opnd = new FolderBrowserDialog();

            if (opnd.ShowDialog() == DialogResult.OK)
            {
                string url = opnd.SelectedPath;              
                DirectoryInfo di = new DirectoryInfo(url);
               
                string filtro = Microsoft.VisualBasic.Interaction.InputBox(
                "Escribe el tipo de archivo con el que se entrenara la base de datos.",
                "Filtro de imagenes.",
                "*.jpg");

                string emocion = Microsoft.VisualBasic.Interaction.InputBox(
                "¡Que emocion se entrenara?",
                "Selección de emoción.",
                "Feliz");

                foreach (var imagen in di.GetFiles(filtro))
                {
                    Console.WriteLine(imagen.FullName);
                    Image<Gray, byte> gray = new Image<Gray, byte>(imagen.FullName);   
                    entrenarBD(gray,emocion);
                }
                MessageBox.Show("La emoción " + emocion + " se a guardado correctamente", "Imagen guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void entrenarConCámaraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string emocion = Microsoft.VisualBasic.Interaction.InputBox(
                "¡Que emocion se entrenara?",
                "Selección de emoción.",
                "Feliz");

                //Trained face counter
                ContTrain = ContTrain + 1;

                //Get a gray frame from capture device
                gray = grabber.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                //Face Detector
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                face,
                1.2,
                10,
                Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(20, 20));

                //Action for each element detected
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                    break;
                }

                //resize face detected image for force to compare the same size with the 
                //test image with cubic interpolation type method
                TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                trainingImages.Add(TrainedFace);
                labels.Add(emocion);

                //Show face added in gray scale
                imageBox1.Image = TrainedFace;

                //Write the number of triained faces in a file text for further load
                File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");
                //File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                //Write the labels of triained faces in a file text for further load
                for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedFaces/face" + i + ".jpg");
                    File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");

                }

                MessageBox.Show("La emoción " + emocion + " se a guardado correctamente", "Imagen guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Primero habilite la detección", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public FrmPrincipal()
        {
            InitializeComponent();
            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels+1; tf++)
                {
                    LoadFaces = "face" + tf + ".jpg";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }
            
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString());
                MessageBox.Show("Nada en la base de datos binaria, por favor agrega por lo menos una expresión(Simplemente entrega el protripo con el Botón Guardar Imagen).", "Procesando base de datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //Initialize the capture device
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber);
            button1.Enabled = false;
        }


        private void button2_Click(object sender, System.EventArgs e)
        {
            
        }


        public void FrameGrabber(object sender, EventArgs e)
        {

            label3.Text = "0";
            //label4.Text = "";
            NamePersons.Add("");


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    //Convert it to Grayscale
                    gray = currentFrame.Convert<Gray, Byte>();

                    //Face Detector
                    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                  face,
                  1.2,
                  10,
                  Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                  new Size(20, 20));

                    //Action for each element detected
                    foreach (MCvAvgComp f in facesDetected[0])
                    {
                        t = t + 1;
                        result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        //draw the face detected in the 0th (gray) channel with blue color
                        currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                        if (trainingImages.ToArray().Length != 0)
                        {
                            //TermCriteria for face recognition with numbers of trained images like maxIteration
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                        //Eigen face recognizer
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                           trainingImages.ToArray(),
                           labels.ToArray(),
                           3000,
                           ref termCrit);

                        name = recognizer.Recognize(result);

                            //Draw the label for each face detected and recognized
                        currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                        }

                            NamePersons[t-1] = name;
                            NamePersons.Add("");


                        //Set the number of faces detected on the scene
                        label3.Text = facesDetected[0].Length.ToString();
                       
                        /*
                        //Set the region of interest on the faces
                        
                        gray.ROI = f.rect;
                        MCvAvgComp[][] eyesDetected = gray.DetectHaarCascade(
                           eye,
                           1.1,
                           10,
                           Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                           new Size(20, 20));
                        gray.ROI = Rectangle.Empty;

                        foreach (MCvAvgComp ey in eyesDetected[0])
                        {
                            Rectangle eyeRect = ey.rect;
                            eyeRect.Offset(f.rect.X, f.rect.Y);
                            currentFrame.Draw(eyeRect, new Bgr(Color.Blue), 2);
                        }
                         */

                    }
                        t = 0;

                        //Names concatenation of persons recognized
                    for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                    {
                        names = names + NamePersons[nnn] + ", ";
                    }
                    //Show the faces procesed and recognized
                    imageBoxFrameGrabber.Image = currentFrame;
                    label4.Text = names;
                    names = "";
                    //Clear the list(vector) of names
                    NamePersons.Clear();

                }

       

    }
}