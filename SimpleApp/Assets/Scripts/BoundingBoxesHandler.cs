using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Barracuda;

using System.IO;
using TFClassify;
using System.Linq;
using System.Collections;


public class BoundingBoxesHandler : MonoBehavior
{ 
    public float shiftX = 0f;
    public float shiftY = 0f;
    public float scaleFactor = 1;

    public Color colorTag = new Color(0.3843137f, 0, 0.9333333f);
    private static GUIStyle labelStyle;
    private static Texture2D boxOutlineTexture;

    // bounding boxes detected for current frame
    private IList<BoundingBox> boxOutlines;

    // bounding boxes detected across frames
    public List<BoundingBox> boxSavedOutlines = new List<BoundingBox>();

    
    public void Initialize()
    {
        this.boxOutlineTexture = new Texture2D(1, 1);
        this.boxOutlineTexture.SetPixel(0, 0, this.colorTag);
        this.boxOutlineTexture.Apply();

        this.labelStyle = new GUIStyle();
        this.labelStyle.fontSize = 50;
        this.labelStyle.normal.textColor = this.colorTag;
    }

    public void UpdateBoundingBoxes()
    {

    }

    public void ClearAll()
    {
        this.boxSavedOutlines.Clear();
        this.boxOutlines.Clear();
    }

    
    // OnGUI is called for rendering and handling GUI events.
    public void DrawBoxes()
    {
        foreach (var outline in this.boxSavedOutlines)
        {
            DrawBoxOutline(outline, scaleFactor, shiftX, shiftY);
        }
    }


    // merging bounding boxes and save result to boxSavedOutlines
    private void GroupBoxOutlines()
    {
        // if savedoutlines is empty, add current frame outlines if possible.
        if (this.boxSavedOutlines.Count == 0)
        {
            // no bounding boxes in current frame
            if (this.boxOutlines == null || this.boxOutlines.Count == 0)
            {
                return;
            }
            // deep copy current frame bounding boxes
            foreach (var outline in this.boxOutlines)
            {
                this.boxSavedOutlines.Add(outline);
            }
            return;
        }

        // adding current frame outlines to existing savedOulines and merge if possible.
        bool addOutline = false;
        foreach (var outline1 in this.boxOutlines)
        {
            bool unique = true;
            foreach (var outline2 in this.boxSavedOutlines)
            {
                // if two bounding boxes are for the same object, use high confidnece one
                if (IsSameObject(outline1, outline2))
                {
                    unique = false;
                    if (outline1.Confidence > outline2.Confidence + 0.05F) //& outline2.Confidence < 0.5F)
                    {
                        Debug.Log("DEBUG: add detected boxes in this frame.");
                        Debug.Log($"DEBUG: Add Label: {outline1.Label}. Confidence: {outline1.Confidence}.");
                        Debug.Log($"DEBUG: Remove Label: {outline2.Label}. Confidence: {outline2.Confidence}.");

                        this.boxSavedOutlines.Remove(outline2);
                        this.boxSavedOutlines.Add(outline1);
                        addOutline = true;
                        staticNum = 0;
                        break;
                    }
                }
            }
            // if outline1 in current frame is unique, add it permanently
            if (unique)
            {
                Debug.Log($"DEBUG: add detected boxes in this frame");
                addOutline = true;
                staticNum = 0;
                this.boxSavedOutlines.Add(outline1);
                Debug.Log($"Add Label: {outline1.Label}. Confidence: {outline1.Confidence}.");
            }
        }
        if (!addOutline)
        {
            staticNum += 1;
        }

        // merge same bounding boxes
        // remove will cause duplicated bounding box?
        List<BoundingBox> temp = new List<BoundingBox>();
        foreach (var outline1 in this.boxSavedOutlines)
        {
            if (temp.Count == 0)
            {
                temp.Add(outline1);
                continue;
            }
            foreach (var outline2 in temp)
            {
                if (IsSameObject(outline1, outline2))
                {
                    if (outline1.Confidence > outline2.Confidence)
                    {
                        temp.Remove(outline2);
                        temp.Add(outline1);
                        Debug.Log("DEBUG: merge bounding box conflict!!!");
                    }
                }
                else
                {
                    temp.Add(outline1);
                }
            }
        }
        this.boxSavedOutlines = temp;
    }

    // For two bounding boxes, if at least one center is inside the other box,
    // treate them as the same object.
    private bool IsSameObject(BoundingBox outline1, BoundingBox outline2)
    {
        var xMin1 = outline1.Dimensions.X * this.scaleFactor + this.shiftX;
        var width1 = outline1.Dimensions.Width * this.scaleFactor;
        var yMin1 = outline1.Dimensions.Y * this.scaleFactor + this.shiftY;
        var height1 = outline1.Dimensions.Height * this.scaleFactor;
        float center_x1 = xMin1 + width1 / 2f;
        float center_y1 = yMin1 + height1 / 2f;

        var xMin2 = outline2.Dimensions.X * this.scaleFactor + this.shiftX;
        var width2 = outline2.Dimensions.Width * this.scaleFactor;
        var yMin2 = outline2.Dimensions.Y * this.scaleFactor + this.shiftY;
        var height2 = outline2.Dimensions.Height * this.scaleFactor;
        float center_x2 = xMin2 + width2 / 2f;
        float center_y2 = yMin2 + height2 / 2f;

        bool cover_x = (xMin2 < center_x1) && (center_x1 < (xMin2 + width2));
        bool cover_y = (yMin2 < center_y1) && (center_y1 < (yMin2 + height2));
        bool contain_x = (xMin1 < center_x2) && (center_x2 < (xMin1 + width1));
        bool contain_y = (yMin1 < center_y2) && (center_y2 < (yMin1 + height1));

        return (cover_x && cover_y) || (contain_x && contain_y);
    }

    private void CalculateShift(int inputSize)
    {
        int smallest;

        if (Screen.width < Screen.height)
        {
            smallest = Screen.width;
            this.shiftY = (Screen.height - smallest) / 2f;
        }
        else
        {
            smallest = Screen.height;
            this.shiftX = (Screen.width - smallest) / 2f;
        }

        this.scaleFactor = smallest / (float)inputSize;
    }


    private void DrawBoxOutline(BoundingBox outline, float scaleFactor, float shiftX, float shiftY)
    {
        var x = outline.Dimensions.X * scaleFactor + shiftX;
        var width = outline.Dimensions.Width * scaleFactor;
        var y = outline.Dimensions.Y * scaleFactor + shiftY;
        var height = outline.Dimensions.Height * scaleFactor;

        DrawRectangle(new Rect(x, y, width, height), 10, this.colorTag);
        DrawLabel(new Rect(x, y - 80, 200, 20), $"Localizing {outline.Label}: {(int)(outline.Confidence * 100)}%");
    }


    public static void DrawRectangle(Rect area, int frameWidth, Color color)
    {
        Rect lineArea = area;
        lineArea.height = frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Top line

        lineArea.y = area.yMax - frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Bottom line

        lineArea = area;
        lineArea.width = frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Left line

        lineArea.x = area.xMax - frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Right line
    }


    private static void DrawLabel(Rect position, string text)
    {
        GUI.Label(position, text, labelStyle);
    }

    private Texture2D Scale(Texture2D texture, int imageSize)
    {
        var scaled = TextureTools.scaled(texture, imageSize, imageSize, FilterMode.Bilinear);
        return scaled;
    }


    private Color32[] Rotate(Color32[] pixels, int width, int height)
    {
        var rotate = TextureTools.RotateImageMatrix(
                pixels, width, height, 90);
        // var flipped = TextureTools.FlipYImageMatrix(rotate, width, height);
        //flipped =  TextureTools.FlipXImageMatrix(flipped, width, height);
        // return flipped;
        return rotate;
    }



}
