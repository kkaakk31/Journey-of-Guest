using System;

namespace JoG.Localization {

    [Serializable]
    public struct LocalizableString {
        public string token;
        public readonly string Value => Localizer.GetString(token);

        public static implicit operator string(in LocalizableString localizableString) => localizableString.Value;

        public static implicit operator LocalizableString(string str) => new() { token = str };

        public readonly string GetValueFormat(object arg0) {
            return Localizer.GetString(token, arg0);
        }

        public readonly string GetValueFormat(object arg0, object arg1) {
            return Localizer.GetString(token, arg0, arg1);
        }

        public readonly string GetValueFormat(object arg0, object arg1, object arg2) {
            return Localizer.GetString(token, arg0, arg1, arg2);
        }

        public readonly string GetValueFormat(params object[] args) {
            return Localizer.GetString(token, args);
        }

        public override readonly string ToString() => Value;
    }
}