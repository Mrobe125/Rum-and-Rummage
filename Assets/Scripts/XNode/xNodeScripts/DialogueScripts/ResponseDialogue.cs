using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class ResponseDialogue : DialogueNodeBase
{
  [Input(typeConstraint = TypeConstraint.Strict)] public int entry;
  [Output(connectionType = ConnectionType.Override)] public bool exit;

  [Header("No Quest if left blank")]
  public Sprite questImage;

  public override string GetDialogueType { get { return "Response"; } }
}