namespace unlight_cli {
    public class ConfigTempContainer {
        public string work_dir_path;
        public ConfigTemp content;
    }
    public class ConfigTemp {
        public string root_path;

    }
    public static class Config {
        public static ConfigTempContainer LoadConfigTemp (string work_dir) {
            var t = new ConfigTempContainer ();
            t.work_dir_path = work_dir;
            
            return t;
        }
    }
}