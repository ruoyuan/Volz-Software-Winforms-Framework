using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using VS.GUI.InheritedControl.Interface;
using VS.Logic;

namespace VS.GUI.InheritedControl {
  public partial class VSDateTime : System.Windows.Forms.DateTimePicker, IDataBindSinglePersist, IStateChangedListener {
    public event EventHandler BindingOccured;
    private void OnBindingOccured(System.EventArgs e) {
      EventHandler ev = BindingOccured;
      if (ev != null) {
        ev(this, e);
      }
    }
    private Color _backgroundColor = Color.White;
    private Color _foregroundColor = Color.Black;
    public VSDateTime() { 
      InitializeComponent();      
      this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.ForeColor = this._foregroundColor;
      this.BackColor = this._backgroundColor;
      this.GotFocus += new System.EventHandler(this._OnGotFocus);
      this.LostFocus += new System.EventHandler(this._OnLostFocus);
      this.Value = DateTime.Now;
    }
    public void _OnGotFocus(object sender, System.EventArgs e) {
      this.ForeColor = Color.Black;
      this.BackColor = Color.FromArgb(215, 244, 215);
    }
    public void _OnLostFocus(object sender, System.EventArgs e) {
      this.ForeColor = this._foregroundColor;
      this.BackColor = this._backgroundColor;
    }
    protected object _DataSource;
    protected string _DataSourceProperty;
    protected string _LocalProperty;
    public object DataSource {
      get { return this._DataSource; }
      set {
        this._DataSource = value;
        this.RebindData();
      }
    }
    public string DataSourceProperty {
      get {
        return this._DataSourceProperty;
      }
      set {
        this._DataSourceProperty = value;
        this.RebindData();
      }
    }
    public string LocalProperty {
      get {
        return this._LocalProperty;
      }
      set {
        this._LocalProperty = value;
        this.RebindData();
      }
    }
    public void RebindData() {
      try {
        this.DataBindings.Clear();
        this.DataBindings.Add(this._LocalProperty, this._DataSource, this._DataSourceProperty);
        OnBindingOccured(EventArgs.Empty);
      }
      catch (Exception) { }
    }
    public void RebindData(object DataSource) {
      try {
        this.DataBindings.Clear();
        RebindData();
      }
      catch (Exception) { }
    }
    public void BindData(string LocalProperty, object DataSource, string DataSourceProperty) {
      this._DataSource = DataSource;
      this._DataSourceProperty = DataSourceProperty;
      this._LocalProperty = LocalProperty;
      this.RebindData();
    }
    public void StateChanged(BaseLogic sender, StateChangedEventArgs args) {
      if (args.State == eState.RETRIEVED) {
        this.DataSource = sender.Container;
      }
    }
  }
}
