﻿using System;
using GalaSoft.MvvmLight;
using SimpleLyricsEditor.ValueConvert;

namespace SimpleLyricsEditor.DAL
{
    public class Lyric : ObservableObject, IComparable<Lyric>
    {
        private string _content;
        private TimeSpan _time;
        private bool _isSelected;

        public Lyric(TimeSpan time)
        {
            _time = time;
        }

        public Lyric(TimeSpan time, string content) : this(time)
        {
            _content = content.Trim();
        }

        public TimeSpan Time
        {
            get => _time;
            set => Set(ref _time, value);
        }

        public string Content
        {
            get => _content;
            set => Set(ref _content, String.IsNullOrWhiteSpace(value) ? String.Empty : value.Trim());
        }
        
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        
        public int CompareTo(Lyric other)
        {
            return this._time.CompareTo(other.Time);
        }

        public override string ToString()
        {
            return $"[{_time.ToLyricTimeString()}] {_content.Trim()}";
        }
    }
}