﻿using HSDRaw.Common;

namespace HSDRaw.MEX
{
    public class MEX_CostumeFileSymbolTable : HSDAccessor
    {
        public override int TrimmedSize => 0x04;

        public HSDArrayAccessor<MEX_CostumeFileSymbol> CostumeSymbols { get => _s.GetReference<HSDArrayAccessor<MEX_CostumeFileSymbol>>(0x00); set => _s.SetReference(0x00, value); }

        public override void New()
        {
            base.New();
            CostumeSymbols = new HSDArrayAccessor<MEX_CostumeFileSymbol>() { _s = new HSDStruct(new byte[4]) };
        }
    }

    public class MEX_CostumeFileSymbol : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public string FileName
        {
            get => FileNameS?.Value;
            set
            {
                if (string.IsNullOrEmpty(value))
                    FileNameS = null;
                else
                {
                    if (FileNameS == null)
                        FileNameS = new HSD_String();
                    FileNameS.Value = value;
                }
            }
        }

        public string JointSymbol
        {
            get => JointS?.Value;
            set
            {
                if(string.IsNullOrEmpty(value))
                    JointS = null;
                else
                {

                    if (JointS == null)
                        JointS = new HSD_String();
                    JointS.Value = value;
                }
            }
        }

        public string MatAnimSymbol
        {
            get => MatS?.Value;
            set
            {
                if (string.IsNullOrEmpty(value))
                    MatS = null;
                else
                {
                    if (MatS == null)
                        MatS = new HSD_String();
                    MatS.Value = value;
                }
            }
        }

        private HSD_String FileNameS { get => _s.GetReference<HSD_String>(0x00); set => _s.SetReference(0x00, value); }

        private HSD_String JointS { get => _s.GetReference<HSD_String>(0x04); set => _s.SetReference(0x04, value); }

        private HSD_String MatS { get => _s.GetReference<HSD_String>(0x08); set => _s.SetReference(0x08, value); }

        public int VisibilityLookupIndex { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        public override string ToString()
        {
            return FileName;
        }
    }
}
