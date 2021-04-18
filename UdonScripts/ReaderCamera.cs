
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReaderCamera : UdonSharpBehaviour
{
    private Camera readerCamera;
    private RenderTexture renderTexture;
    public Texture2D readerTexture;
    [UdonSynced] public string lastRead;

    public int blockSize = 20;
    public float readerDelay = 1.0f;

    void Start()
    {
        readerCamera = gameObject.GetComponent<Camera>();
        renderTexture = readerCamera.targetTexture;
    }

    private void OnPostRender()
    {
        if (readerDelay >= 0)
        {
            readerDelay -= Time.deltaTime;
            return;
        }
        else
        {
            readerDelay = 1.0f;
        }

        readerTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readerTexture.Apply();

        string binaryStr = ReadPixelsToBinaryString(readerTexture, blockSize, renderTexture.width, renderTexture.height);
        string interpretedStr = BinaryToString(StringToByteArray(binaryStr));
        lastRead = interpretedStr;
    }

    /// <summary>
    /// Simple string to byte array converter
    /// </summary>
    public byte[] StringToByteArray(string input)
    {
        int numOfBytes = input.Length / 8;
        byte[] bytes = new byte[numOfBytes];
        for (int i = 0; i < numOfBytes; ++i)
        {
            bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
        }

        return bytes;
    }

    /// <summary>
    /// Get string from byte array
    /// </summary>
    /// <param name="bytes">byte array</param>
    /// <returns></returns>
    public string BinaryToString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length];
        Array.Copy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    /// <summary>
    /// Reads the video screen to binary
    /// </summary>
    private string ReadPixelsToBinaryString(Texture2D texture, int blockSize = 1, int width = 1920, int height = 1080)
    {
        string binaryStr = string.Empty;

        width = texture.width < width ? texture.width : width;
        height = texture.height < height ? texture.height : height;

        for (int y = height; y >= 0; y -= blockSize)
        {
            for (int x = 0; x < width; x += blockSize)
            {
                int yCenter = y - (blockSize / 2);
                int xCenter = x + (blockSize / 2);

                Color pixelColor = texture.GetPixel(xCenter, yCenter);

                if (pixelColor.r < 0.1 && pixelColor.g < 0.1 && pixelColor.b < 0.1) // black
                {
                    binaryStr += '0';
                }
                else if (pixelColor.r > 0.6 && pixelColor.r <= 1.0 && pixelColor.b < 0.1 && pixelColor.g < 0.1) // red
                {
                    y = 0;
                    break; // stop reading as we've reached an end marker
                }
                else // white
                {
                    binaryStr += '1';
                }
            }
        }

        return binaryStr;
    }
}
