using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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

        public void addSlide(Slide s)
        {
            slides.Add(s);
        }

        private Slide getSlide(int index)
        {
            return slides.ElementAt(index);
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




    }


    public class Slide
    {
        //Need to add video support as well.
        private Image backgroundImage;
        private List<Slide> associatedSlides;
        private List<Animation> animations;
        private Slide parentSlide;

        public Slide(String imagePath)
        {
            backgroundImage = Image.FromFile(imagePath);
            associatedSlides = new List<Slide>();
            animations = new List<Animation>();
        }

        public void addSlide(Slide s)
        {
            associatedSlides.Add(s);
        }

        public void removeSlide(Slide s)
        {
            associatedSlides.Remove(s);
        }

        public void setParent(Slide s)
        {
            parentSlide = s;
        }

        public void setAssociation(Slide s)
        {
            associatedSlides.Add(s);
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

        public Image getImage()
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


