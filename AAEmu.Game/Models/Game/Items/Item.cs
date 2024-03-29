﻿using System;

using AAEmu.Commons.Network;
using AAEmu.Game.Models.Game.Items.Containers;
using AAEmu.Game.Models.Game.Items.Templates;

namespace AAEmu.Game.Models.Game.Items;

public class Item : PacketMarshaler, IComparable<Item>
{
    private byte _worldId;
    private ulong _ownerId;
    private ulong _id;
    private uint _templateId;
    private SlotType _slotType;
    private int _slot;
    private byte _grade;
    private ItemFlag _itemFlags;
    private int _count;
    private int _lifespanMins;
    private uint _madeUnitId;
    private DateTime _createTime;
    private DateTime _unsecureTime;
    private DateTime _unpackTime;
    private uint _imageItemTemplateId;
    private bool _isDirty;
    private ulong _uccId;
    private DateTime _expirationTime;
    private double _expirationOnlineMinutesLeft;
    private DateTime _chargeUseSkillTime;
    private byte _flags;
    private byte _durability;
    private short _chargeCount;
    private ushort _TemperPhysical;
    private ushort _TemperMagical;
    private uint _runeId;
    private DateTime _chargeTime;

    public bool IsDirty { get => _isDirty; set => _isDirty = value; }
    public byte WorldId { get => _worldId; set { _worldId = value; _isDirty = true; } }
    public ulong OwnerId { get => _ownerId; set { _ownerId = value; _isDirty = true; } }
    public ulong Id { get => _id; set { _id = value; _isDirty = true; } }
    public uint TemplateId { get => _templateId; set { _templateId = value; _isDirty = true; } }
    public ItemTemplate Template { get; set; }
    public virtual uint DetailBytesLength { get; } = 0;
    public SlotType SlotType { get => _slotType; set { _slotType = value; _isDirty = true; } }
    public int Slot { get => _slot; set { _slot = value; _isDirty = true; } }
    public byte Grade { get => _grade; set { _grade = value; _isDirty = true; } }
    public ItemFlag ItemFlags { get => _itemFlags; set { _itemFlags = value; _isDirty = true; } }
    public int Count { get => _count; set { _count = value; _isDirty = true; } }
    public int LifespanMins { get => _lifespanMins; set { _lifespanMins = value; _isDirty = true; } }
    public uint MadeUnitId { get => _madeUnitId; set { _madeUnitId = value; _isDirty = true; } }
    public DateTime CreateTime { get => _createTime; set { _createTime = value; _isDirty = true; } }
    public DateTime UnsecureTime { get => _unsecureTime; set { _unsecureTime = value; _isDirty = true; } }
    public DateTime UnpackTime { get => _unpackTime; set { _unpackTime = value; _isDirty = true; } }
    public uint ImageItemTemplateId { get => _imageItemTemplateId; set { _imageItemTemplateId = value; _isDirty = true; } }

    /// <summary>
    /// Internal representation of the exact time a item will expire (UTC)
    /// </summary>
    public DateTime ExpirationTime
    {
        get => _expirationTime;
        set
        {
            if (_expirationTime != value)
            {
                _expirationTime = value;
                _isDirty = true;
            }
        }
    }

    /// <summary>
    /// Internal representation of the time this item has left before expiring, only counting down if the owning character is online
    /// </summary>
    public double ExpirationOnlineMinutesLeft
    {
        get => _expirationOnlineMinutesLeft;
        set
        {
            _expirationOnlineMinutesLeft = value;
            _isDirty = true;
        }
    }

    public ulong UccId
    {
        get => _uccId;
        set
        {
            _uccId = value;
            if (value > 0)
                SetFlag(ItemFlag.HasUCC);
            else
                RemoveFlag(ItemFlag.HasUCC);
            _isDirty = true;
        }
    }

    public DateTime ChargeStartTime { get; set; } = DateTime.MinValue;
    public virtual ItemDetailType DetailType { get; set; } // TODO 1.0 max type: 8, at 1.2 max type 9, at 3.0.3.0 max type 10, at 3.5.0.3 max type 12
    public DateTime ChargeUseSkillTime { get => _chargeUseSkillTime; set { _chargeUseSkillTime = value; _isDirty = true; } }
    public byte Flags { get => _flags; set { _flags = value; _isDirty = true; } }
    public byte Durability { get => _durability; set { _durability = value; _isDirty = true; } }
    public short ChargeCount { get => _chargeCount; set { _chargeCount = value; _isDirty = true; } }
    public DateTime ChargeTime { get => _chargeTime; set { _chargeTime = value; _isDirty = true; } }
    public ushort TemperPhysical { get => _TemperPhysical; set { _TemperPhysical = value; _isDirty = true; } }
    public ushort TemperMagical { get => _TemperMagical; set { _TemperMagical = value; _isDirty = true; } }
    public uint RuneId { get => _runeId; set { _runeId = value; _isDirty = true; } }

    public uint[] GemIds { get; set; }
    public byte[] Detail { get; set; }

    // Helper
    public ItemContainer _holdingContainer { get; set; }

    public static uint Coins { get; } = 500;
    public static uint TaxCertificate { get; } = 31891;
    public static uint BoundTaxCertificate { get; } = 31892;
    public static uint AppraisalCertificate { get; } = 28085;
    public static uint CrestStamp { get; } = 17662;
    public static uint CrestInk { get; } = 17663;
    public static uint SheetMusic { get; } = 28051;
    public static uint SalonCertificate { get; } = 30811;

    /// <summary>
    /// Sort will use itemSlot numbers
    /// </summary>
    /// <param name="otherItem"></param>
    /// <returns></returns>
    public int CompareTo(Item otherItem)
    {
        if (otherItem == null) return 1;
        return this.Slot.CompareTo(otherItem.Slot);
    }

    public Item()
    {
        WorldId = AppConfiguration.Instance.Id;
        OwnerId = 0;
        Slot = -1;
        _holdingContainer = null;
        _isDirty = true;
        GemIds = new uint[16];
    }

    public Item(byte worldId)
    {
        WorldId = worldId;
        OwnerId = 0;
        Slot = -1;
        _holdingContainer = null;
        _isDirty = true;
        GemIds = new uint[16];
    }

    public Item(ulong id, ItemTemplate template, int count)
    {
        WorldId = AppConfiguration.Instance.Id;
        OwnerId = 0;
        Id = id;
        TemplateId = template.Id;
        Template = template;
        Count = count;
        Slot = -1;
        _holdingContainer = null;
        _isDirty = true;
        GemIds = new uint[16];
    }

    public Item(byte worldId, ulong id, ItemTemplate template, int count)
    {
        WorldId = worldId;
        OwnerId = 0;
        Id = id;
        TemplateId = template.Id;
        Template = template;
        Count = count;
        Slot = -1;
        _holdingContainer = null;
        _isDirty = true;
        GemIds = new uint[16];
    }

    public override void Read(PacketStream stream)
    {
        TemplateId = stream.ReadUInt32();
        if (TemplateId != 0)
        {
            Id = stream.ReadUInt64();
            Grade = stream.ReadByte();
            ItemFlags = (ItemFlag)stream.ReadByte();
            Count = stream.ReadInt32();

            DetailType = (ItemDetailType)stream.ReadByte();
            ReadDetails(stream);

            CreateTime = stream.ReadDateTime();
            LifespanMins = stream.ReadInt32();
            MadeUnitId = stream.ReadUInt32();
            WorldId = stream.ReadByte();
            UnsecureTime = stream.ReadDateTime();
            UnpackTime = stream.ReadDateTime();
            ChargeUseSkillTime = stream.ReadDateTime(); // added in 1.7
        }
    }

    public override PacketStream Write(PacketStream stream)
    {
        stream.Write(TemplateId); // type
        if (TemplateId != 0)
        {
            stream.Write(Id);    // id
            stream.Write(Grade); // grade
            stream.Write((byte)ItemFlags); // flags | bounded
            stream.Write(Count); // stackSize

            stream.Write((byte)DetailType); // detailType
            WriteDetails(stream);

            stream.Write(CreateTime);
            stream.Write(LifespanMins);
            stream.Write(MadeUnitId);
            stream.Write(WorldId);
            stream.Write(UnsecureTime);
            stream.Write(UnpackTime);
            stream.Write(ChargeUseSkillTime); // added in 1.7
        }

        return stream;
    }

    public virtual void ReadDetails(PacketStream stream)
    {
        var mDetailLength = 0;
        switch (DetailType)
        {
            case ItemDetailType.Equipment: // 1
                mDetailLength = 56; // есть расшифровка в items/EquipItem
                break;
            case ItemDetailType.Slave: // 2
                mDetailLength = 30;
                break;
            case ItemDetailType.Mate: // 3
                mDetailLength = 7; // есть расшифровка в items/Summon
                break;
            case ItemDetailType.Ucc: // 4
                mDetailLength = 10; // есть расшифровка в items/UccItem
                break;
            case ItemDetailType.Treasure: // 5
            case ItemDetailType.Location: // 11
                mDetailLength = 25;
                break;
            case ItemDetailType.BigFish: // 6
            case ItemDetailType.Decoration: // 7
                mDetailLength = 17; // есть расшифровка в items/BigFish
                break;
            case ItemDetailType.MusicSheet: // 8
                mDetailLength = 9; // есть расшифровка в items/MusicSheetItem
                break;
            case ItemDetailType.Glider: // 9
                mDetailLength = 5;
                break;
            case ItemDetailType.SlaveEquipment: // 10
                mDetailLength = 13;
                break;
            case ItemDetailType.TypeMax:
            case ItemDetailType.Invalid:
            default:
                break;
        }

        mDetailLength -= 1;
        if (mDetailLength > 0)
        {
            Detail = stream.ReadBytes(mDetailLength);
        }
    }

    public virtual void WriteDetails(PacketStream stream)
    {
        var mDetailLength = 0;
        switch (DetailType)
        {
            case ItemDetailType.Equipment:
                mDetailLength = 56; // есть расшифровка в items/Equipment
                break;
            case ItemDetailType.Slave:
                mDetailLength = 30;
                break;
            case ItemDetailType.Mate:
                mDetailLength = 7; // есть расшифровка в items/Summon
                break;
            case ItemDetailType.Ucc:
                mDetailLength = 10; // есть расшифровка в items/UccItem
                break;
            case ItemDetailType.Treasure:
            case ItemDetailType.Location: // нет в 1.2
                mDetailLength = 25;
                break;
            case ItemDetailType.BigFish: // есть расшифровка в items/BigFish
            case ItemDetailType.Decoration:
                mDetailLength = 17;
                break;
            case ItemDetailType.MusicSheet:
                mDetailLength = 9; // есть расшифровка в items/MusicSheetItem
                break;
            case ItemDetailType.Glider:
                mDetailLength = 5;
                break;
            case ItemDetailType.SlaveEquipment: // нет в 1.2
                mDetailLength = 13;
                break;
            default:
                break;
        }
        mDetailLength -= 1;
        if (mDetailLength > 0)
        {
            Detail = new byte[mDetailLength];
            stream.Write(Detail);
        }
    }

    public virtual bool HasFlag(ItemFlag flag)
    {
        return (ItemFlags & flag) == flag;
    }

    public virtual void SetFlag(ItemFlag flag)
    {
        ItemFlags |= flag;
    }

    public virtual void RemoveFlag(ItemFlag flag)
    {
        ItemFlags &= ~flag;
    }
}
