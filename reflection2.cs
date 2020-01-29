        PropertyInfo piInstance = examType.GetProperty("InstanceProperty");
        piInstance.SetValue(exam, 37);
        
         Assembly assembly = Assembly.LoadFile(@"....bin\Debug\TestCases.dll");
       //get all types
        var testTypes = from t in assembly.GetTypes()
                        let attributes = t.GetCustomAttributes(typeof(NUnit.Framework.TestFixtureAttribute), true)
                        where attributes != null && attributes.Length > 0
                        orderby t.Name
                        select t;

        foreach (var type in testTypes)
        {
            //get test method in types.
            var testMethods = from m in type.GetMethods()
                              let attributes = m.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), true)
                              where attributes != null && attributes.Length > 0
                              orderby m.Name
                              select m;

            foreach (var method in testMethods)
            {
                MethodInfo methodInfo = type.GetMethod(method.Name);

                if (methodInfo != null)
                {
                    object result = null;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);

                    if (parameters.Length == 0)
                    {
                        // This works fine
                        result = methodInfo.Invoke(classInstance, null);
                    }
                    else
                    {
                        object[] parametersArray = new object[] { "Hello" };

                        // The invoke does NOT work;
                        // it throws "Object does not match target type"             
                        result = methodInfo.Invoke(classInstance, parametersArray);
                    }
                }

            }
        }
        
        // get private fields
        FieldInfo[] fields = myType.GetFields(
                         BindingFlags.NonPublic | 
                         BindingFlags.Instance);
                         
         typeof(MyType).GetField("fieldName", BindingFlags.NonPublic | BindingFlags.Instance)
         
         internal static object GetInstanceField(Type type, object instance, string fieldName)
          {
              BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                  | BindingFlags.Static;
              FieldInfo field = type.GetField(fieldName, bindFlags);
              return field.GetValue(instance);
          }
