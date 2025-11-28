using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HardwareCommNet.CommTable;

/// <summary>
///�豸ͨѶ����ά������/����в��ṩ�̰߳�ȫ��������ѯ������ڡ�
/// </summary>
public sealed class CommTable
{
 private readonly List<CommCell> _inputs = new();
 private readonly List<CommCell> _outputs = new();
 private readonly ReaderWriterLockSlim _lock = new();

 public IReadOnlyList<CommCell> Inputs
 {
  get
  {
   _lock.EnterReadLock();
   try
   {
    return _inputs.Select(Clone).ToList();
   }
   finally
   {
    _lock.ExitReadLock();
   }
  }
 }

 public IReadOnlyList<CommCell> Outputs
 {
  get
  {
   _lock.EnterReadLock();
   try
   {
    return _outputs.Select(Clone).ToList();
   }
   finally
   {
    _lock.ExitReadLock();
   }
  }
 }

 public void Clear()
 {
  _lock.EnterWriteLock();
  try
  {
   _inputs.Clear();
   _outputs.Clear();
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 public void ClearInputs()
 {
  _lock.EnterWriteLock();
  try
  {
   _inputs.Clear();
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 public void ClearOutputs()
 {
  _lock.EnterWriteLock();
  try
  {
   _outputs.Clear();
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 public void AddOrUpdateInput(CommCell cell) => AddOrUpdate(_inputs, cell);
 public void AddOrUpdateOutput(CommCell cell) => AddOrUpdate(_outputs, cell);
 public void RemoveInputAt(int index) => RemoveAt(_inputs, index);
 public void RemoveOutputAt(int index) => RemoveAt(_outputs, index);
 public void MoveInput(int index, int offset) => Move(_inputs, index, offset);
 public void MoveOutput(int index, int offset) => Move(_outputs, index, offset);

 /// <summary>
 /// 更新输入变量的缓存值（由轮询线程调用）
 /// </summary>
 public void UpdateInputCachedValue(string name, object value)
 {
  if (string.IsNullOrWhiteSpace(name)) return;
  _lock.EnterWriteLock();
  try
  {
   var cell = _inputs.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
   if (cell != null)
   {
    cell.CachedValue = value;
    cell.CachedTime = DateTime.Now;
   }
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 /// <summary>
 /// 获取输入变量的缓存值
 /// </summary>
 public object GetInputCachedValue(string name)
 {
  if (string.IsNullOrWhiteSpace(name)) return null;
  _lock.EnterReadLock();
  try
  {
   var cell = _inputs.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
   return cell?.CachedValue;
  }
  finally
  {
   _lock.ExitReadLock();
  }
 }

 private void AddOrUpdate(List<CommCell> list, CommCell cell)
 {
  if (cell == null || string.IsNullOrWhiteSpace(cell.Name)) return;
  _lock.EnterWriteLock();
  try
  {
   var idx = list.FindIndex(c => string.Equals(c.Name, cell.Name, System.StringComparison.OrdinalIgnoreCase));
   if (idx >= 0) list[idx] = cell;
   else list.Add(cell);
   Reindex(list);
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 private void RemoveAt(List<CommCell> list, int index)
 {
  _lock.EnterWriteLock();
  try
  {
   if (index >= 0 && index < list.Count)
   {
    list.RemoveAt(index);
    Reindex(list);
   }
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 private void Move(List<CommCell> list, int index, int offset)
 {
  _lock.EnterWriteLock();
  try
  {
   int newIdx = index + offset;
   if (index < 0 || index >= list.Count || newIdx < 0 || newIdx >= list.Count) return;
   var item = list[index];
   list.RemoveAt(index);
   list.Insert(newIdx, item);
   Reindex(list);
  }
  finally
  {
   _lock.ExitWriteLock();
  }
 }

 private static void Reindex(List<CommCell> list)
 {
  for (int i = 0; i < list.Count; i++) list[i].Index = i + 1;
 }

 private static CommCell Clone(CommCell c) => new CommCell
 {
  Index = c.Index,
  Name = c.Name,
  ValueType = c.ValueType,
  StartByte = c.StartByte,
  Length = c.Length,
  Address = c.Address,
  TriggerValues = new List<string>(c.TriggerValues),
  Description = c.Description,
  IsTrigger = c.IsTrigger,
  CachedValue = c.CachedValue,
  CachedTime = c.CachedTime
 };
}