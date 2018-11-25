using System;
using UnityEngine;

[Serializable]
public class SerializableColor {

    public float R = 1;
    public float G = 1;
    public float B = 1;
    public float A = 1;

    public SerializableColor() { }

    public SerializableColor(Color color) {
        R = color.r;
        G = color.g;
        B = color.b;
        A = color.a;
    }

    public Color GetColor() {
        return new Color(R, G, B, A);
    }

}