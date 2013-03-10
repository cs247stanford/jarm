using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Xaml;
using System.Text;
using System.Diagnostics;
//using System.Windows.Freezable;
//using System.Drawing;
//using Microsoft.DirectX.AudioVideoPlayback;




namespace KinectPresentor
{

    static class Constants
    {
        public const double TOLERANCE = 15; // tolerance around the point where we accept valid objects.

    }


    public class Presentation
    {
        /*
         * The presentation class encapsulates what a presentation is and the different navigations that can occur on a presentation
         */
        private List<Slide> slides;
        private int currIndex;

        public Presentation()
        {
            slides = new List<Slide>();
            currIndex = 0;

        }

        public List<Slide> retrieveSlides()
        {
            return slides;
        }

        public void addSlide(Slide s)
        {
            slides.Add(s);
        }

        public int getCurrentIndex()
        {
            return currIndex;
        }

        public Slide getSlide(int index)
        {
            int actualIndex = index % this.slides.Count();
            if (actualIndex < 0)
            {
                actualIndex += this.slides.Count();
            }
            Debug.Assert(actualIndex < this.slides.Count(), "Index is within bounds of path array");
            return slides.ElementAt(actualIndex);
        }

        public Slide getCurrentSlide()
        {
            return getSlide(this.currIndex);
        }

        public Slide getPreviousSlide()
        {
            return getSlide(this.currIndex - 1);
        }

        public Slide getNextSlide()
        {
            return getSlide(this.currIndex + 1);
        }

        public Slide moveToNextSlide()
        {
            this.currIndex += 1;
            return getSlide(this.currIndex);        
        }


        public Slide moveToPreviousSlide()
        {
            this.currIndex -= 1;
            return getSlide(this.currIndex);
        }

        public Slide jumpToSlide(int index)
        {
            int actualIndex = index % this.slides.Count();
            if (actualIndex < 0)
            {
                actualIndex += this.slides.Count();
            }
            this.currIndex = actualIndex;
            return getSlide(this.currIndex);
        }
    }


    public class Slide
    {
        //Need to add video support as well.
        private BitmapImage backgroundImage;
        //private String imageFileName;
        private List<Slide> associatedSlides;
        private List<Animation> animations;
        private Slide parentSlide;
        private String imagePath;
        private bool isVideoSlide = false;
        private String videoPath;
        private int index;
        private static int counter = 0;

        public Slide(String ip)
        {
            if (ip.Substring(ip.Length - 3).ToLower() == "wmv")
            {
                this.isVideoSlide = true;
                this.videoPath = ip;
                this.imagePath = "C:\\Users\\Roger Chen\\Documents\\GitHub\\jarm\\SwipeShow\\bin\\Debug\\Pictures\\blank.jpg";  //"bin\\Debug\\Pictures\\blank.jpg";
            }
            else
            {
                this.isVideoSlide = false;
                this.videoPath = null;
                this.imagePath = ip;
                Debug.WriteLine(ip);

            }
            backgroundImage = new BitmapImage(new Uri(this.imagePath));
            associatedSlides = new List<Slide>();
            animations = new List<Animation>();
            index = counter;
            counter++;
        }

        public bool hasVideo()
        {
            return this.isVideoSlide;
        }

        public String getVideoPath()
        {
            return this.videoPath;
        }

        public int getIndex()
        {
            return index;
        }

        public void addSlide(Slide s)
        {
            associatedSlides.Add(s);
        }

        public void removeSlide(Slide s)
        {
            associatedSlides.Remove(s);
        }

        public String getImagePath()
        {
            return this.imagePath;
        }

        public void setParent(Slide s)
        {
            parentSlide = s;
        }

        public void setAssociation(Slide s)
        {
            associatedSlides.Add(s);
        }

        public void addAssociatedSlides(List<Slide> slides)
        {
            foreach (Slide s in slides)
            {
                associatedSlides.Add(s);
            }
        }

        public Slide getAssociation(int index)
        {
            return associatedSlides.ElementAt(index);
        }


        public void addAnimation(Animation a) 
        {
            animations.Add(a);
        }


        public int isAnimationPresent(int x, int y) 
        {
            for (int i=0 ; i < animations.Count() ; i++)  {
                if(animations.ElementAt(i).isMatch(x,y)){
                    return i;
                }
            }
            return -1;
        }

        public Animation getAnimation(int index)
        {
            if (index == -1) 
                return null;
            return animations.ElementAt(index);

        }

        public List<Animation> getAllAnims()
        {
            return animations;
        }

        public List<Slide> getAllAssociated()
        {
            return associatedSlides;
        }

        public BitmapImage getImage()
        {
            return backgroundImage;
        }
    }

    public class Animation {

        private int initialXPos;
        private int initialYPos;
        private int finalXPos;
        private int finalYPos;
        private String animationType;


        public Animation(int initX, int initY, int finalX, int finalY , String anim)
        {
            initialXPos = initX;
            initialYPos = initY;
            finalXPos = finalX;
            finalYPos = finalY;
            animationType = anim;
        }

        public int getX() {
            return initialXPos;
        }

        public int getY() {
        
            return initialYPos;
        }

        public int getFinalX()
        {

            return finalXPos;
        }


        public int getFinalY()
        {
            return finalYPos;
        }

        public bool isMatch(int x, int y)
        {
            if (x < (initialXPos + Constants.TOLERANCE) && x > (initialXPos-Constants.TOLERANCE)) {
                if(y < (initialYPos + Constants.TOLERANCE) && y > (initialYPos-Constants.TOLERANCE)) {
                    return true;
                }
            }
            return false;
        }


        public String getType()
        {
            return animationType;
        }


    }



}


