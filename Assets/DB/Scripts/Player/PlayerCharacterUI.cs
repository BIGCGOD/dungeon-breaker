/// <summary>
/// Player character UI.
/// Basic Player UI will show up any information of Player Character
/// Such as Inventory System , Skill System , etc...
/// </summary>

using UnityEngine;
using System.Collections;


public class PlayerCharacterUI : MonoBehaviour {

	public GUISkin skin;
	private CharacterInventory character;
	private CharacterSkillDeployer skill;
	private Vector2 scrollPosition;

	enum UIState
	{
		None	= 0,
		Item	= 1,
	}

	UIState uiState	= UIState.None;
	void Start ()
	{	
		character	= this.gameObject.GetComponent<CharacterInventory>();	
		skill	= this.gameObject.GetComponent<CharacterSkillDeployer>();	
	}

	void Update()
	{
		if(Screen.lockCursor && Input.GetKey(KeyCode.E))
			uiState	= UIState.Item;

		Screen.lockCursor	= uiState == UIState.None;
	}
	
	
	void OnGUI()
	{
		if(skin)
			GUI.skin = skin;

		if(character)
		{
			GUI.skin.label.fontSize = 18;
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(60,Screen.height-130,300,30),"Equiped");
			GUI.BeginGroup(new Rect(50,Screen.height-100,Screen.width,100));
			for(int i=0;i<character.ItemsEquiped.Length;i++){
				if(character.ItemsEquiped[i]!=null){
					DrawItemBox(character.ItemsEquiped[i],new Vector2(i*60,0));
				}
			}
			GUI.EndGroup();
			
			
			if(uiState == UIState.Item)
			{
				Screen.lockCursor = false;
				GUI.BeginGroup(new Rect(Screen.width - 330,30,300,350));
				GUI.skin.label.fontSize = 20;
				GUI.skin.label.alignment = TextAnchor.UpperLeft;
				GUI.Label(new Rect(10,10,150,30),"Item Lists");
			
				scrollPosition = GUI.BeginScrollView(new Rect(0, 50, 300, 300), scrollPosition, new Rect(0, 50, 280, character.ItemSlots.Count * 50));
				for(int i=0;i<character.ItemSlots.Count;i++){
					DrawItemBoxDetail(character.ItemSlots[i],new Vector2(0,(i*60) + 50));
				}
				GUI.EndScrollView();
			
				if(GUI.Button(new Rect(270,0,30,30),"X"))
					uiState	= UIState.None;

				GUI.EndGroup();
			}else{
				GUI.skin.label.fontSize = 17;
				GUI.skin.label.alignment = TextAnchor.UpperRight;
				GUI.Label(new Rect(Screen.width-330,30,300,30),"Press 'E' Open Inventory");	
			}
		}
		
		
		if(skill)
		{
			for(int i=0;i<skill.Skill.Length;i++)
			{
				DrawSkill(i,new Vector2((Screen.width - (skill.Skill.Length * 60) - 30) + i*60,Screen.height - 100));
			}	
		}
	}
	
	// Draw skill icon
	void DrawSkill(int index,Vector2 position)
	{
		if(skill.indexSkill == index)
		{
			GUI.skin.label.fontSize = 13;
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(10 + position.x,position.y - 10,55,50),"Selected");
		}

		if(GUI.Button(new Rect(10 + position.x,10 + position.y,50,50),skill.SkillIcon[index])){
			skill.indexSkill = index;
		}
		
	}
	
	// Draw item icon
	void DrawItemBox(ItemSlot itemslot,Vector2 position){
		if(itemslot!=null){
			ItemCollector item = character.itemManager.Items[itemslot.Index];
			GUI.Box(new Rect(10 + position.x,10 + position.y,50,50),"");	
			GUI.DrawTexture(new Rect(10 + position.x,10 + position.y,50,50),item.Icon);
			GUI.skin.label.fontSize = 13;
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(14+position.x,14+position.y,30,30),itemslot.Num.ToString());
		}
	}
	
	
	// Draw Item icon with detail
	void DrawItemBoxDetail(ItemSlot itemslot,Vector2 position){
		if(itemslot!=null){
			var item = character.itemManager.Items[itemslot.Index];
			GUI.Box(new Rect(10 + position.x,10 + position.y,50,50),"");	
			GUI.DrawTexture(new Rect(10 + position.x,10 + position.y,50,50),item.Icon);
			GUI.skin.label.fontSize = 13;
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(14+position.x,14+position.y,30,30),itemslot.Num.ToString());
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(position.x+70,position.y,100,60),item.Name);
			
			switch(item.ItemType)
			{
			case ItemType.Weapon:
				if(character.CheckEquiped(itemslot)){
					if(GUI.Button(new Rect(200 + position.x, position.y+10,80,30),"UnEquipped")){
						character.UnEquipItem(itemslot);
					}
				}else{
					if(GUI.Button(new Rect(200 + position.x, position.y+10,80,30),"Equip")){
						character.EquipItem(itemslot);
					}
				}
				break;
			case ItemType.Edible:
				if(GUI.Button(new Rect(200 + position.x, position.y+10,80,30),"Use")){
					character.UseItem(itemslot);
				}
				break;
				
			}
		}
	}
	
}
