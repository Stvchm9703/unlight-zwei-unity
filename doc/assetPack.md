# Protocol Documentation
<a name="top"></a>

## Table of Contents

- [Protocol Documentation](#protocol-documentation)
  - [Table of Contents](#table-of-contents)
  - [doc/assetPack.proto](#docassetpackproto)
    - [Remark](#remark)
    - [CardObject](#cardobject)
    - [CardSetPack](#cardsetpack)
    - [ImgSet](#imgset)
    - [MultiLang](#multilang)
    - [SkillObject](#skillobject)
  - [Scalar Value Types](#scalar-value-types)
  
- [Scalar Value Types](#scalar-value-types)



<a name="doc/assetPack.proto"></a>
<p align="right"><a href="#top">Top</a></p>

## doc/assetPack.proto

### Remark 
This proto file is not used in running code, it just used for generating Object Class and UML 
in System Documentation. 

Suppose the Character Card Asset (**CCAsset**) should contain those in list before build
```yaml
# this information refer to Unity AssetBundle 
# ...
Assets:
- Assets/Data/cc01/card_set.json
- Assets/Data/cc01/cover_b.png
- Assets/Data/cc01/cover_1.png
- Assets/Data/cc01/cover_d.png
- Assets/Data/cc01/cover_e.png
- Assets/Data/cc01/skill.json
- Assets/Data/cc01/skill_0_1.png
- Assets/Data/cc01/skill_1_1.png
- Assets/Data/cc01/cover_4.png
- Assets/Data/cc01/skill_0_shad.png
- Assets/Data/cc01/cover_c.png
- Assets/Data/cc01/skill_2_1.png
- Assets/Data/cc01/cover_3.png
- Assets/Data/cc01/cover_a.png
- Assets/Data/cc01/skill_3_parts_3.png
- Assets/Data/cc01/skill_3_parts_4.png
- Assets/Data/cc01/stand_1.png
- Assets/Data/cc01/stand_f.png
- Assets/Data/cc01/skill_1_front.png
- Assets/Data/cc01/cover_5.png
- Assets/Data/cc01/cover_f.png
- Assets/Data/cc01/skill_3_parts_2.png
- Assets/Data/cc01/cover_2.png
- Assets/Data/cc01/skill_1_shad.png
- Assets/Data/cc01/skill_3_1.png
- Assets/Data/cc01/stand_shad.png
- Assets/Data/cc01/skill_3_parts_5.png
- Assets/Data/cc01/skill_2_shad.png
Dependencies: []
```

by this list of asset, it builds as CC`{CC_id}`.ab in `Asset/streamingAsset`
- `card_set.json` is json file contain [CardObject](###CardObject) 's Dataset.
- `skill.json` is json file contain list of [SkillObject](#skillobject)'s Dataset.
- `cover_{number}.png` is PNG file for Charecter Card Cover. 
- `skill_{number}.png` is PNG file for Charecter Skill Animation. Some of Asset with `skill_{number}_{number}.png` means the skill contain mutliple image for one animation.
- `stand_1.png` is Character Stand image. 
- `stand_shad.png` is Character Stand shadow image.
- `stand_f.png` is Character close shot image, during the phase `move_card_drop`, `attack_card_drop` and `defence_card_drop`

<a name=".CardObject"></a>

### CardObject
this class is for packing and unpacking the Character information from Json file (`card_set.json`)
Character as main subject of dataset
Since the name and nickname (ab_name) is repeated in relation table from origanal project. 



| Field    | Type                        | Label    | Description                                      |
| -------- | --------------------------- | -------- | ------------------------------------------------ |
| id       | [int32](#int32)             |          | Character dataset's id                           |
| card_set | [CardSetPack](#CardSetPack) | repeated | list of Character card                           |
| name     | [MultiLang](#MultiLang)     |          | the translation map of Character's name          |
| ab_name  | [MultiLang](#MultiLang)     |          | the translation map of Character's Nickname      |
| caption  | [MultiLang](#MultiLang)     |          | the translation map of Character's caption title |






<a name=".CardSetPack"></a>

### CardSetPack


| Field             | Type                        | Label    | Description                                                                                    |
| ----------------- | --------------------------- | -------- | ---------------------------------------------------------------------------------------------- |
| id                | [int32](#int32)             |          | Card Set's id                                                                                  |
| level             | [int32](#int32)             |          | level of Character Card                                                                        |
| hp                | [int32](#int32)             |          | health point                                                                                   |
| ap                | [int32](#int32)             |          | attack point                                                                                   |
| dp                | [int32](#int32)             |          | defence point                                                                                  |
| rarity            | [int32](#int32)             |          | rarity in drawing box (origanal game data)                                                     |
| deck_cost         | [int32](#int32)             |          | cost value in Character Deck                                                                   |
| slot              | [int32](#int32)             |          | slot in drawing box (origanal game data)                                                       |
| stand_image       | [ImgSet](#ImgSet)           |          | raw image information of stand image (`stand_1.png`) before Unity Assetbundle's packing        |
| stand_image_t2    | [Texture2D](#Texture2D)     |          | Unity Texture2D of stand image, it should be `null` after unpacked from Assetbundle            |
| chara_image       | [ImgSet](#ImgSet)           |          | raw image information of cover image (`cover_{number}.png`) before Unity Assetbundle's packing |
| chara_image_t2    | [Texture2D](#Texture2D)     |          | Unity Texture2D of cover image, it should be `null` after unpacked from Assetbundle            |
| artifact_image    | [ImgSet](#ImgSet)           |          | raw image information of close shot image (`stand_f.png`) before Unity Assetbundle's packing   |
| artifact_image_t2 | [Texture2D](#Texture2D)     |          | Unity Texture2D of close shot image, it should be `null` after unpacked from Assetbundle       |
| bg_image          | [ImgSet](#ImgSet)           |          | raw image information of stand shadow image (`stand_f.png`) before Unity Assetbundle's packing |
| bg_image_2        | [Texture2D](#Texture2D)     |          | Unity Texture2D of stand shadow image, it should be `null` after unpacked from Assetbundle     |
| next_id           | [int32](#int32)             |          | id of next level card set  (origanal game data)                                                |
| kind              | [int32](#int32)             |          | card kind ( 0: character, 1: monster )                                                         |
| created_at        | [string](#string)           |          | date of created                                                                                |
| skill             | [SkillObject](#SkillObject) | repeated | list of Skill Object from `skill.json`                                                         |
| skill_pointer     | [int32](#int32)             | repeated | list of skill object's id                                                                      |






<a name=".ImgSet"></a>

### ImgSet

File information for image type, 
since the Unity AssetBundle compress and deform the image size.

| Field  | Type              | Label | Description                       |
| ------ | ----------------- | ----- | --------------------------------- |
| name   | [string](#string) |       | the file name in AssetBundle      |
| height | [int32](#int32)   |       | height of image                   |
| widt   | [int32](#int32)   |       | width of image                    |
| type   | [string](#string) |       | image type (e.g : PNG, Tiff, Jpg) |






<a name=".MultiLang"></a>

### MultiLang
Dataset of  Translation Map 

| Field | Type              | Label | Description          |
| ----- | ----------------- | ----- | -------------------- |
| jp    | [string](#string) |       | Japanese             |
| en    | [string](#string) |       | Engish               |
| fr    | [string](#string) |       | French               |
| kr    | [string](#string) |       | Korean               |
| scn   | [string](#string) |       | Simplied-Chinese     |
| tcn   | [string](#string) |       | Tranditional-Chinese |
| ina   | [string](#string) |       | India                |
| thai  | [string](#string) |       | Thai                 |



<a name=".SkillObject"></a>

### SkillObject
Dataset of Skill Object in `skill.json`


| Field           | Type                    | Label | Description                                                                                    |
| --------------- | ----------------------- | ----- | ---------------------------------------------------------------------------------------------- |
| id              | [int32](#int32)         |       | id of skill object                                                                             |
| feat_no         | [int32](#int32)         |       | skill function's id                                                                            |
| pow             | [int32](#int32)         |       | power value of skill object                                                                    |
| dice_attribute  | [string](#string)       |       | dice result execution (origanal game data)                                                     |
| effect_image    | [ImgSet](#ImgSet)       |       | raw image information of skill image (`skill_{number}.png`) before Unity Assetbundle's packing |
| effect_image_t2 | [Texture2D](#Texture2D) |       | Unity Texture2D of skill image, it should be `null` after unpacked from Assetbundle            |
| condition       | [string](#string)       |       | trigger condition                                                                              |
| created_at      | [string](#string)       |       | data created time                                                                              |
| name            | [MultiLang](#MultiLang) |       | skill name                                                                                     |
| caption         | [MultiLang](#MultiLang) |       | skill detail information                                                                       |








## Scalar Value Types

| .proto Type                    | Notes                                                                                                                                           | C++    | Java       | Python      | Go      | C#         | PHP            | Ruby                           |
| ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------- | ------ | ---------- | ----------- | ------- | ---------- | -------------- | ------------------------------ |
| <a name="double" /> double     |                                                                                                                                                 | double | double     | float       | float64 | double     | float          | Float                          |
| <a name="float" /> float       |                                                                                                                                                 | float  | float      | float       | float32 | float      | float          | Float                          |
| <a name="int32" /> int32       | Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint32 instead. | int32  | int        | int         | int32   | int        | integer        | Bignum or Fixnum (as required) |
| <a name="int64" /> int64       | Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint64 instead. | int64  | long       | int/long    | int64   | long       | integer/string | Bignum                         |
| <a name="uint32" /> uint32     | Uses variable-length encoding.                                                                                                                  | uint32 | int        | int/long    | uint32  | uint       | integer        | Bignum or Fixnum (as required) |
| <a name="uint64" /> uint64     | Uses variable-length encoding.                                                                                                                  | uint64 | long       | int/long    | uint64  | ulong      | integer/string | Bignum or Fixnum (as required) |
| <a name="sint32" /> sint32     | Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int32s.                            | int32  | int        | int         | int32   | int        | integer        | Bignum or Fixnum (as required) |
| <a name="sint64" /> sint64     | Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int64s.                            | int64  | long       | int/long    | int64   | long       | integer/string | Bignum                         |
| <a name="fixed32" /> fixed32   | Always four bytes. More efficient than uint32 if values are often greater than 2^28.                                                            | uint32 | int        | int         | uint32  | uint       | integer        | Bignum or Fixnum (as required) |
| <a name="fixed64" /> fixed64   | Always eight bytes. More efficient than uint64 if values are often greater than 2^56.                                                           | uint64 | long       | int/long    | uint64  | ulong      | integer/string | Bignum                         |
| <a name="sfixed32" /> sfixed32 | Always four bytes.                                                                                                                              | int32  | int        | int         | int32   | int        | integer        | Bignum or Fixnum (as required) |
| <a name="sfixed64" /> sfixed64 | Always eight bytes.                                                                                                                             | int64  | long       | int/long    | int64   | long       | integer/string | Bignum                         |
| <a name="bool" /> bool         |                                                                                                                                                 | bool   | boolean    | boolean     | bool    | bool       | boolean        | TrueClass/FalseClass           |
| <a name="string" /> string     | A string must always contain UTF-8 encoded or 7-bit ASCII text.                                                                                 | string | String     | str/unicode | string  | string     | string         | String (UTF-8)                 |
| <a name="bytes" /> bytes       | May contain any arbitrary sequence of bytes.                                                                                                    | string | ByteString | str         | []byte  | ByteString | string         | String (ASCII-8BIT)            |

