using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioImp))]
public class AudioImpEditor : Editor
{
    private VisualElement datosContainer;
    private TextField nombreField;
    private DropdownField metodoDeReproduccionDropdown;
    private DropdownField eventosDropdown;
    private Button agregarButton;
    private ScrollView listaEventosContainer;
    private VisualElement listaContainer;
    private Toggle startToggle;
    private ObjectField gameobjectField;
    private DropdownField tipoDropdown;
    private DropdownField verTipoDropdown;
    private Dictionary<string, List<string>> configuracionMetodos;

    public static class AudioImpEditorState
    {
        public static string TipoSeleccionado;
        public static string NombreSeleccionado;
        public static string MetodoSeleccionado;
        public static string EventoSeleccionado;
        public static GameObject ObjetoSeleccionado;
        public static bool StartSeleccionado;
        public static string VerTipoSeleccionado = "M�sica";
    }

    public override VisualElement CreateInspectorGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/AM_FMOD/AudioImpInterfaz.uxml");
        var root = visualTree.CloneTree();

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/AM_FMOD/AudioImpStyle.uss");
        root.styleSheets.Add(styleSheet);

        datosContainer = root.Q<VisualElement>("Datos");

        tipoDropdown = root.Q<DropdownField>("tipoDrop");
        tipoDropdown.choices = AudioImp.TiposEvento;
        tipoDropdown.value = AudioImpEditorState.TipoSeleccionado ?? tipoDropdown.value;
        tipoDropdown.RegisterValueChangedCallback(evt => AudioImpEditorState.TipoSeleccionado = evt.newValue);

        verTipoDropdown = root.Q<DropdownField>("verTipo");
        verTipoDropdown.choices = AudioImp.TiposEvento;
        verTipoDropdown.value = AudioImpEditorState.VerTipoSeleccionado;
        verTipoDropdown.RegisterValueChangedCallback(evt =>
        {
            AudioImpEditorState.VerTipoSeleccionado = evt.newValue;
            ActualizarListaEventosFiltrada(evt.newValue);
        });

        nombreField = root.Q<TextField>("nombreElegido");
        nombreField.value = AudioImpEditorState.NombreSeleccionado ?? nombreField.value;
        nombreField.RegisterValueChangedCallback(evt => AudioImpEditorState.NombreSeleccionado = evt.newValue);

        metodoDeReproduccionDropdown = root.Q<DropdownField>("metodosDropdown");
        metodoDeReproduccionDropdown.choices = new List<string>(FMODEvents.ObtenerConfiguracionMetodos().Keys);
        metodoDeReproduccionDropdown.value = AudioImpEditorState.MetodoSeleccionado ?? metodoDeReproduccionDropdown.value;
        metodoDeReproduccionDropdown.RegisterValueChangedCallback(evt =>
        {
            AudioImpEditorState.MetodoSeleccionado = evt.newValue;
            ActualizarCamposVisibles(evt.newValue);
        });

        eventosDropdown = root.Q<DropdownField>("eventosDropdown");
        eventosDropdown.choices = FMODEvents.instance.ObtenerTodosLosEventos();
        eventosDropdown.value = AudioImpEditorState.EventoSeleccionado ?? eventosDropdown.value;
        eventosDropdown.RegisterValueChangedCallback(evt => AudioImpEditorState.EventoSeleccionado = evt.newValue);

        startToggle = root.Q<Toggle>("startToggle");
        startToggle.value = AudioImpEditorState.StartSeleccionado;
        startToggle.RegisterValueChangedCallback(evt => AudioImpEditorState.StartSeleccionado = evt.newValue);

        gameobjectField = root.Q<ObjectField>("object");
        gameobjectField.objectType = typeof(GameObject);
        gameobjectField.value = AudioImpEditorState.ObjetoSeleccionado;
        gameobjectField.RegisterValueChangedCallback(evt => AudioImpEditorState.ObjetoSeleccionado = (GameObject)evt.newValue);

        agregarButton = root.Q<Button>("agregarButton");
        agregarButton.clicked += AgregarEvento;

        listaEventosContainer = root.Q<ScrollView>("listaEventosContainer");
        listaContainer = root.Q<VisualElement>("Lista");

        configuracionMetodos = FMODEvents.ObtenerConfiguracionMetodos();
        ActualizarCamposVisibles(metodoDeReproduccionDropdown.value);
        ActualizarListaEventosFiltrada(verTipoDropdown.value);

        var citaLabel = new Label("By: Qpa85")
        {
            style =
            {
                unityTextAlign = TextAnchor.MiddleRight,
                fontSize = 10,
                unityFontStyleAndWeight = FontStyle.Italic,
                color = Color.gray,
                marginTop = 10,
                marginBottom = 5,
                alignSelf = Align.FlexEnd
            }
        };
        root.Add(citaLabel);

        return root;
    }


    private void ActualizarCamposVisibles(string metodoSeleccionado)
    {
        datosContainer.Clear();
        datosContainer.Add(tipoDropdown);
        datosContainer.Add(nombreField);
        datosContainer.Add(eventosDropdown);
        datosContainer.Add(metodoDeReproduccionDropdown);

        if (configuracionMetodos.ContainsKey(metodoSeleccionado))
        {
            foreach (var campo in configuracionMetodos[metodoSeleccionado])
            {
                if (campo == "transformField") datosContainer.Add(gameobjectField);
            }

            foreach (var campo in configuracionMetodos[metodoSeleccionado])
            {
                if (campo == "startToggle") datosContainer.Add(startToggle);
            }
        }
    }

    private void AgregarEvento()
    {
        var audioImp = (AudioImp)target;
        if (string.IsNullOrEmpty(nombreField.value) || string.IsNullOrEmpty(eventosDropdown.value)) return;

        if (audioImp.Eventos.Exists(e => e.NombreElegido == nombreField.value)) return;

        var nuevoEvento = new Evento
        {
            NombreElegido = nombreField.value,
            Metodo = metodoDeReproduccionDropdown.value,
            EventoFMOD = eventosDropdown.value,
            gameobject = (GameObject)gameobjectField.value,
            start = startToggle.value,
            Tipo = tipoDropdown.value
        };

        audioImp.AgregarEvento(nuevoEvento);
        EditorUtility.SetDirty(audioImp);
        verTipoDropdown.value = tipoDropdown.value;
        ActualizarListaEventosFiltrada(tipoDropdown.value);
    }

    private void ActualizarListaEventosFiltrada(string tipoSeleccionado)
{
    listaEventosContainer.Clear();
    var audioImp = (AudioImp)target;
    var eventosFiltrados = audioImp.ObtenerEventosPorTipo(tipoSeleccionado);
    listaEventosContainer.style.display = eventosFiltrados.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

    foreach (var evento in eventosFiltrados)
    {
            var eventoItem = new VisualElement();

            // Asignar clase seg�n la categor�a seleccionada
            switch (tipoSeleccionado)
            {
                case "M�sica":
                    eventoItem.AddToClassList("evento-item-musica");
                    break;
                case "SFX":
                    eventoItem.AddToClassList("evento-item-sfx");
                    break;
                case "Ambiente":
                    eventoItem.AddToClassList("evento-item-ambiente");
                    break;
                default:
                    eventoItem.AddToClassList("evento-item"); // Estilo general por defecto
                    break;
            }
            var nombreEditable = new TextField("Nombre") { value = evento.NombreElegido };
        nombreEditable.RegisterValueChangedCallback(evt =>
        {
            evento.NombreElegido = evt.newValue;
            EditorUtility.SetDirty(audioImp);
        });
        eventoItem.Add(nombreEditable);

            var eventoFMODEditable = new DropdownField("Eventos FMOD", eventosDropdown.choices, evento.EventoFMOD);
            eventoFMODEditable.RegisterValueChangedCallback(evt =>
            {
                evento.EventoFMOD = evt.newValue;
                EditorUtility.SetDirty(audioImp);
            });
            eventoItem.Add(eventoFMODEditable);

            var metodoEditable = new DropdownField("M�todo", new List<string>(configuracionMetodos.Keys), evento.Metodo);
        metodoEditable.RegisterValueChangedCallback(evt =>
        {
            evento.Metodo = evt.newValue;
            EditorUtility.SetDirty(audioImp);
            ActualizarListaEventosFiltrada(tipoSeleccionado); // Actualizar lista al cambiar el m�todo
        });
        eventoItem.Add(metodoEditable);

        var dynamicFieldsContainer = new VisualElement();
        dynamicFieldsContainer.AddToClassList("dynamic-fields-container");

        if (configuracionMetodos.ContainsKey(evento.Metodo))
        {
            foreach (var campo in configuracionMetodos[evento.Metodo])
            {
                switch (campo)
                {
                    case "transformField":
                        var transformEditable = new ObjectField("Reproducir Desde")
                        {
                            objectType = typeof(GameObject),
                            value = evento.gameobject
                        };
                        transformEditable.RegisterValueChangedCallback(evt =>
                        {
                            evento.gameobject = (GameObject)evt.newValue;
                            EditorUtility.SetDirty(audioImp);
                        });
                        dynamicFieldsContainer.Add(transformEditable);
                        break;
                }
            }

            foreach (var campo in configuracionMetodos[evento.Metodo])
            {
                switch (campo)
                {
                    case "startToggle":
                        var startEditable = new Toggle("Reproducir") { value = evento.start };
                        startEditable.RegisterValueChangedCallback(evt =>
                        {
                            evento.start = evt.newValue;
                            EditorUtility.SetDirty(audioImp);
                        });
                        dynamicFieldsContainer.Add(startEditable);
                        break;
                }
            }

        }

        eventoItem.Add(dynamicFieldsContainer);

        var eliminarButton = new Button(() =>
        {
            audioImp.Eventos.Remove(evento);
            EditorUtility.SetDirty(audioImp);
            ActualizarListaEventosFiltrada(tipoSeleccionado);
        })
        {
            text = "Eliminar"
        };
        eventoItem.Add(eliminarButton);

        listaEventosContainer.Add(eventoItem);
    }
}

}
