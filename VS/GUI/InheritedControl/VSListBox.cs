using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using VS.Container;
using VS.GUI.InheritedControl.Interface;
using VS.Logic;
using VS.Logic.Filter;

namespace VS.GUI.InheritedControl {
  public partial class VSListBox : System.Windows.Forms.ListBox, IStateChangedListener, IDataBindListPersist {
    protected IListLogicFilter filter = null;
    public IListLogicFilter Filter {
      get { return this.filter; }
      set { this.filter = value; }
    }
    private List<BaseContainer> _DataSourceList;
    public List<BaseContainer> DataSourceList {
      get {
        return this._DataSourceList;
      }
      set {
        this._DataSourceList = value;
        this.RebindData();
      }
    }
    private List<BaseContainer> _AlternativeDataSourceList;
    public List<BaseContainer> AlternativeDataSourceList {
      get { return this._AlternativeDataSourceList; }
      set {
        this._AlternativeDataSourceList = value;
        this.RebindData();
      }
    }
    //private BaseLogic _AlternativeLogic;
    //public BaseLogic AlternativeLogic {
    //  get { return this._AlternativeLogic; }
    //  set { 
    //    this._AlternativeLogic = value;
    //    if( _AlternativeLogic != null &&
    //        _AlternativeLogic.Containers != null)
    //      this._DataSourceList = this._AlternativeLogic.Containers;
    //    this.RebindData();
    //  }
    //}
    protected bool showNotAssigned = false;
    public bool ShowNotAssigned {
      get { return this.showNotAssigned; }
      set { showNotAssigned = value; }
    }
    protected bool JustAdded = false;
    protected bool JustDeleted = false;    
    public VSListBox() {
      this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, 
        System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.BackColor = System.Drawing.Color.White;
      filter = new PassThroughFilter();
    }
    public void AddItems(List<BaseContainer> List) {
      this.Items.Clear();
      this.Items.AddRange(List.ToArray());
      
    }
    protected int GetIndex(int id) {
      if (this.Items == null)
        return -1;

      for (int i = 0; i < this.Items.Count; i++) {
        if (((BaseContainer)this.Items[i]).ID == id)
          return i;
      }
      return -1;
    }
    protected int GetMaxIndex() {
      if (this.Items == null)
        return -1;

      int maxID = -1;
      int index = 0;
      for (int i = 0; i < this.Items.Count; i++) {
        if (((BaseContainer)this.Items[i]).ID > maxID) {
          maxID = ((BaseContainer)this.Items[i]).ID;
          index = i;
        }
      }
      return index;
    }
    protected int GetPreviousIndex(int PreviousID) {
      if (this.Items != null &&
        this.Items.Count > 0) {
        if (PreviousID <= 0) { return 0; }
        else {          
          int i = 0;
          foreach (BaseContainer container in this.Items) {
            if (container.ID == PreviousID) {
              if (i == 0) return 0;
              else return i - 1;
            }
            i++;
          }
          return 0;
        }
      }
      return -1;
    }
    protected bool EnableSelectedIndexChanged;
    protected override void OnSelectedIndexChanged(EventArgs e) {
      if (this.EnableSelectedIndexChanged)
        base.OnSelectedIndexChanged(e);
    }
    public void BindData(List<BaseContainer> DataSourceList) {
      this._DataSourceList = DataSourceList;
      //this.RebindData();
    }
    public void RebindData() {
      try {
        //this.DataBindings.Clear();
        //this.DataBindings.Add(this._LocalProperty, this._DataSource, this._DataSourceProperty);
        //this.DataSource = this._DataSource;
        //this.ValueMember = "ID";
        //this.DisplayMember = "NAME";
        //BaseListContainer blc = (BaseListContainer)this.DataSource;
        List<BaseContainer> targetList = this._AlternativeDataSourceList!=null ? _AlternativeDataSourceList : this._DataSourceList;
        
        this.EnableSelectedIndexChanged = false;
        if (showNotAssigned) {
          List<BaseContainer> containers = FilterList(targetList);
          this.AddItems(containers);
        }
        else {

          List<BaseContainer> shallowCopy = targetList.GetRange(0, targetList.Count);
          for (int i = 0; i < shallowCopy.Count; i++) {
            if (shallowCopy[i].ID == 0) {
              shallowCopy.RemoveAt(i);             
              break;
            }
          }
          List<BaseContainer> containers = FilterList(shallowCopy);
          this.AddItems(containers);
        }
        this.EnableSelectedIndexChanged = true;
        this.OnItemsChanged(EventArgs.Empty);
      }
      catch (Exception) {
        //System.Windows.Forms.MessageBox.Show( ex.ToString() );
      }
    }
    public void StateChanged(BaseLogic sender, StateChangedEventArgs args) {
      if (args.State == eState.DELETING) {
        this.JustDeleted = true;

      }

      if (args.State == eState.ADDED) {
        JustAdded = true;

      }

      if (args.State == eState.RETRIEVED) {
        int NextID = -1;
        if (this.SelectedItem != null)
          NextID = ((BaseContainer)this.SelectedItem).ID;
        if (this.JustDeleted)
          NextID = this.GetPreviousIndex(NextID);

        this.EnableSelectedIndexChanged = false;
        this._DataSourceList = sender.Containers;
        this.RebindData();
        this.EnableSelectedIndexChanged = true;

        if (this.JustAdded) {
          this.SelectedIndex = this.GetMaxIndex();
          this.JustAdded = false;
          return;
        }
        if (this.JustDeleted) {
          this.SelectedIndex = NextID;
          this.JustDeleted = false;
          return;
        }

        if (this.Items != null &&
          this.Items.Count > 0) {

          if (NextID < 0) {
            this.SelectedIndex = 0;
          }
          else {
            int index = this.GetIndex(NextID);
            if (index < 0)
              this.SelectedIndex = 0;
            else
              this.SelectedIndex = index;
          }
        }
      }
    }
    public event EventHandler ItemsChanged;
    protected void OnItemsChanged(EventArgs args) {
      EventHandler ev = this.ItemsChanged;
      if (ev != null) {
        ev(this, args);
      }
    }
    protected List<BaseContainer> FilterList(List<BaseContainer> containers) {
      List<BaseContainer> copy = new List<BaseContainer>();
      foreach (BaseContainer container in containers) {
        if (filter.Filter(container)) copy.Add(container);
      }
      return copy;
    }
  }
  
}
