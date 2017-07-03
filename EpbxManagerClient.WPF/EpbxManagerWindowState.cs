using EpbxManagerClient.Atendimento;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpbxManagerClient
{
    internal class WindowState : INotifyPropertyChanged
    {
        private bool _isLogado;
        private bool _isEmChamada;
        private ChamadaInfo _chamadaAtiva;
        private IntervaloInfo _intervaloInfoAtivo;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IntervaloInfo> IntervaloInfoList { get; set; } = new ObservableCollection<IntervaloInfo>();

        public bool IsLogado
        {
            get { return _isLogado; }
            set { _isLogado = value; ChangedLivre(nameof(IsLogado)); }
        }

        public bool IsEmChamada
        {
            get { return _isEmChamada; }
            set { _isEmChamada = value; ChangedLivre(nameof(IsEmChamada)); }
        }

        public bool IsLivre
        {
            get { return _isLogado && !_isEmChamada; }
        }

        public ChamadaInfo ChamadaAtiva
        {
            get { return _chamadaAtiva; }
            set { _chamadaAtiva = value; ChangedEmChamada(nameof(ChamadaAtiva)); }
        }

        public IntervaloInfo IntervaloInfoAtivo
        {
            get { return _intervaloInfoAtivo; }
            set { _intervaloInfoAtivo = value; Changed(nameof(IntervaloInfoAtivo)); }
        }

        public string EmChamadaCom
        {
            get { return $"Em chamada com {_chamadaAtiva?.Telefone}"; }
        }

        private void Changed(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChangedLivre(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLivre)));
        }

        private void ChangedEmChamada(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmChamadaCom)));
        }
    }
}
