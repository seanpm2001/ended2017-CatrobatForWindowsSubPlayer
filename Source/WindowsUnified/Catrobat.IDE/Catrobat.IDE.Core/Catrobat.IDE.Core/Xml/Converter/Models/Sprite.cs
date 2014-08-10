﻿using System.Collections.Generic;
using System.Linq;
using Catrobat.IDE.Core.ExtensionMethods;
using Catrobat.IDE.Core.Xml.Converter;
using Catrobat.IDE.Core.Xml.XmlObjects;
using Catrobat.IDE.Core.Xml.XmlObjects.Scripts;
using Context = Catrobat.IDE.Core.Xml.Converter.XmlProgramConverter.ConvertBackContext;

// ReSharper disable once CheckNamespace
namespace Catrobat.IDE.Core.Models
{
    partial class Sprite : IXmlObjectConvertibleCyclic<XmlSprite, Context>
    {
        XmlSprite IXmlObjectConvertibleCyclic<XmlSprite, Context>.ToXmlObject(Context context, bool pointerOnly)
        {
            // prevents endless loops
            XmlSprite result;
            if (!context.Sprites.TryGetValue(this, out result))
            {
                result = new XmlSprite {Name = Name};
                context.Sprites[this] = result;
            }
            if (pointerOnly) return result;

            result.Looks = new XmlLookList
            {
                Looks = context.Looks == null ? new List<XmlLook>() : context.Looks.Values.ToList()
            };
            result.Sounds = new XmlSoundList
            {
                Sounds = context.Sounds == null ? new List<XmlSound>() : context.Sounds.Values.ToList()
            };
            result.Scripts = new XmlScriptList
            {
                Scripts = Scripts == null
                    ? new List<XmlScript>()
                    : Scripts.Select(script => script.ToXmlObject(context)).ToList()
            };
            return result;
        }
    }
}
