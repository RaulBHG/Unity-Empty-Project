using System;
using UnityEngine;
using System.Collections.Generic;
using TFF.Salvatore.SaveLoadSystem.Core;
using TFF.Salvatore.SaveLoadSystem.Utils;
using UnityEngine.UIElements;

namespace TFF.Salvatore.Test
{
    public class ColorPositionSaveElement : ASaveElement
    {
        private SpriteRenderer _spriteRenderer;
        

        // Used to save information provided by this object
        public override List<DataChunk> OnSave() 
        {
            dataList = new()
            {
                SavePosition(),
                SaveColor(),
            };
            return dataList;
        }

        // Used to load information provided by this object
        public override void OnLoad(List<DataChunk> data) {
            Guid positionGuid = GuidService.GenerateGuidFromString("Positions");
            Guid colorGuid = GuidService.GenerateGuidFromString("Color");
            data.ForEach(chunk =>
            {
                if (chunk.guid == positionGuid) LoadPosition(chunk);
                if (chunk.guid == colorGuid)    LoadColor(chunk);
            });
        }

        /*******************/
        /* Private methods */
        /*******************/
        private DataChunk SavePosition()
        {
            var hola = transform.position.x;
            var toString = hola.ToString();
            DataChunk position = new();
            position.guid = GuidService.GenerateGuidFromString("Positions");
            position.data = new()
            {
                { "x", transform.position.x.ToString() },
                { "y", transform.position.y.ToString() }
            };

            return position;
        }
        private DataChunk SaveColor()
        {
            Color color = _spriteRenderer.color;
            DataChunk colorChunk = new();
            colorChunk.guid = GuidService.GenerateGuidFromString("Color");
            colorChunk.data = new()
            {
                { "r", color.r.ToString() },
                { "g", color.g.ToString() },
                { "b", color.b.ToString() },
                { "a", color.a.ToString() }
            };

            return colorChunk;
        }
        private void LoadPosition(DataChunk chunk)
        {
            float x = float.Parse((string)chunk.data["x"]);
            float y = float.Parse((string)chunk.data["y"]);
            transform.position = new Vector3(x, y);
        }
        private void LoadColor(DataChunk chunk)
        {
            float r = float.Parse((string)chunk.data["r"]);
            float g = float.Parse((string)chunk.data["g"]);
            float b = float.Parse((string)chunk.data["b"]);
            float a = float.Parse((string)chunk.data["a"]);
            _spriteRenderer.color = new Color(r, g, b, a);
            //_spriteRenderer.color = (Color)chunk.data["color"];
        }

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


    }
}
