
Please generate a ui-config.json for a lab automation dialog with the following specifications:

**Important**: Pay close attention to visual design. Ensure all fonts and colors provide strong contrast against the application's theme colors for readability. Also, pay attention to dimensions; the window's size cannot be too small, even if there is little content. And, do not add comments in the generated config file.

1.  **Window Title**: Set the window title to "AiDialogDemo".
2.  **Data Export**: Configure the root 'dataExport' object to:
    * Save to the file: "C:\Users\Public\VerisExe\sample_run.csv"
    * Use the "exportdata_and_close" action.
    * Include headers.
    * Use 'overwrite' writeMode.
3.  **Controls**: Create a "Grid" layout for the following parameters. For each parameter, create a Label and an appropriate input control (TextBox or CheckBox).
    * The control's 'name' property MUST exactly match the 'CSV_Header'.
    * The 'text' (for TextBox) or 'isChecked' (for CheckBox) should be set based on 'Init_Value'.
4.  **Data Mappings**: Create a 'dataExport.mappings' entry for every parameter, mapping 'columnHeader' (from 'CSV_Header') to 'sourceControlName' (also the 'CSV_Header').

**Parameters to create:**

-   **Parameter**: "ParaString"
    -   **Control Type**: "TextBox"
    -   **Control Name**: "ParaString"
    - "text": "\"\""
    -   **Data Mapping**: Map column "ParaString" to control "ParaString".

-   **Parameter**: "ParaInt"
    -   **Control Type**: "CheckBox"
    -   **Control Name**: "ParaInt"
    - "isChecked": false
    -   **Data Mapping**: Map column "ParaInt" to control "ParaInt".

-   **Parameter**: "ParaFloat"
    -   **Control Type**: "TextBox"
    -   **Control Name**: "ParaFloat"
    - "text": "0.0"
    -   **Data Mapping**: Map column "ParaFloat" to control "ParaFloat".

-   **Parameter**: "EnableParaBoolen"
    -   **Control Type**: "CheckBox"
    -   **Control Name**: "EnableParaBoolen"
    - "isChecked": false
    -   **Data Mapping**: Map column "EnableParaBoolen" to control "EnableParaBoolen".

Finally, add a "Button" at the bottom of the grid with the text "Continue" and the action "exportdata_and_close".
