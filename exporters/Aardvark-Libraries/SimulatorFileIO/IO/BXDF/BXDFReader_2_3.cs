using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

public partial class BXDFProperties
{
    #region XSD Markup

    /// <summary>
    /// The XSD markup to ensure valid document reading.
    /// </summary>
    private const string BXDF_XSD_2_3 =
        @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>

        <!-- definition of simple elements -->

        <xs:element name='X' type='xs:decimal'/>
        <xs:element name='Y' type='xs:decimal'/>
        <xs:element name='Z' type='xs:decimal'/>
        <xs:element name='W' type='xs:decimal'/>
        <xs:element name='Separated' type='xs:boolean'/>
        <xs:element name='Mass' type='xs:decimal'/>
        <xs:element name='SubMeshID' type='xs:integer'/>
        <xs:element name='CollisionMeshID' type='xs:integer'/>
        <xs:element name='PropertySetID' type='xs:string'/>
        <xs:element name='Scale' type='xs:decimal'/>
        <xs:element name='Convex' type='xs:boolean'/>

        <xs:element name='Friction'>
            <xs:simpleType>
                <xs:restriction base='xs:decimal'>
                    <xs:minInclusive value='0'/>
                    <xs:maxInclusive value='100'/>
                </xs:restriction>
            </xs:simpleType>
        </xs:element>

        <!-- definition of attributes -->

        <xs:attribute name='GUID' type='xs:string'/>
        <xs:attribute name='ID' type='xs:string'/>

        <xs:attribute name='Version'>
            <xs:simpleType>
                <xs:restriction base='xs:string'>
                    <xs:pattern value='2\.3\.\d+'/>
                </xs:restriction>
            </xs:simpleType>
        </xs:attribute>

        <!-- definition of complex elements -->

        <xs:element name='BXDVector3'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='X'/>
                    <xs:element ref='Y'/>
                    <xs:element ref='Z'/>
                </xs:sequence>
                <xs:attribute ref='ID' use='required'/>
            </xs:complexType>
        </xs:element>

        <xs:element name='BXDQuaternion'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='X'/>
                    <xs:element ref='Y'/>
                    <xs:element ref='Z'/>
                    <xs:element ref='W'/>
                </xs:sequence>
                <xs:attribute ref='ID' use='required'/>
            </xs:complexType>
        </xs:element>

        <xs:element name='NodeGroup'>
            <xs:complexType>
                <xs:sequence>
                    <xs:choice minOccurs='0' maxOccurs='unbounded'>
                    <xs:element ref='NodeGroup'/>
                    <xs:element ref='Node'/>
                    </xs:choice>
                </xs:sequence>
                <xs:attribute ref='ID' use='required'/>
            </xs:complexType>
        </xs:element>

        <xs:element name='BoxCollider'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='BXDVector3'/>
                </xs:sequence>
            </xs:complexType>
        </xs:element>

        <xs:element name='SphereCollider'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='Scale'/>
                </xs:sequence>
            </xs:complexType>
        </xs:element>

        <xs:element name='MeshCollider'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='Convex'/>
                </xs:sequence>
            </xs:complexType>
        </xs:element>

        <xs:element name='Joint'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='BXDVector3'/>
                    <xs:element ref='BXDVector3'/>
                </xs:sequence>
            </xs:complexType>
        </xs:element>

        <xs:element name='PropertySet'>
            <xs:complexType>
                <xs:sequence>
                    <xs:choice minOccurs='0' maxOccurs='1'>
                        <xs:element ref='BoxCollider'/>
                        <xs:element ref='SphereCollider'/>
                        <xs:element ref='MeshCollider'/>
                    </xs:choice>
                    <xs:element ref='Separated'/>
                    <xs:element ref='Friction'/>
                    <xs:element ref='Mass'/>
                    <xs:element ref='Joint' minOccurs='0' maxOccurs='1'/>
                </xs:sequence>
                <xs:attribute ref='ID' use='required'/>
            </xs:complexType>
        </xs:element>

        <xs:element name='Node'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='BXDVector3'/>
                    <xs:element ref='BXDQuaternion'/>
                    <xs:element ref='SubMeshID'/>
                    <xs:element ref='CollisionMeshID' minOccurs='0' maxOccurs='1'/>
                    <xs:element ref='PropertySetID' minOccurs='0' maxOccurs='1'/>
                </xs:sequence>
                <xs:attribute ref='ID' use='required'/>
            </xs:complexType>
        </xs:element>

        <xs:element name='BXDF'>
            <xs:complexType>
                <xs:sequence>
                    <xs:element ref='PropertySet' minOccurs='0' maxOccurs='unbounded'/>
                    <xs:element ref='NodeGroup'/>
                </xs:sequence>
                <xs:attribute ref='Version' use='required'/>
                <xs:attribute ref='GUID' use='required'/>
            </xs:complexType>
        </xs:element>

        </xs:schema>";

    #endregion

    /// <summary>
    /// Reads the properties contained in the XML BXDF file specified and returns
    /// the corresponding FieldDefinition.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="useValidation"></param>
    /// <returns></returns>
    private static FieldDefinition ReadProperties_2_3(string path, out string result, bool useValidation = true)
    {
        // The FieldDefinition to be returned.
        FieldDefinition fieldDefinition = null;

        XmlReaderSettings settings = new XmlReaderSettings();

        if (useValidation)
        {
            settings.Schemas.Add(XmlSchema.Read(new StringReader(BXDF_XSD_2_3), null));
            settings.ValidationType = ValidationType.Schema;
        }
        else
        {
            settings.ValidationType = ValidationType.None;
        }

        XmlReader reader = XmlReader.Create(path, settings);

        try
        {
            foreach (string name in IOUtilities.AllElements(reader))
            {
                switch (name)
                {
                    case "BXDF":
                        // Assign a value to fieldDefinition with the given GUID attribute.
                        fieldDefinition = FieldDefinition.Factory(new Guid(reader["GUID"]));
                        break;
                    case "PropertySet":
                        // Reads the current element as a PropertySet.
                        ReadPropertySet_2_3(reader.ReadSubtree(), fieldDefinition);
                        break;
                    case "NodeGroup":
                        // Reads the root FieldNodeGroup.
                        ReadFieldNodeGroup_2_2(reader.ReadSubtree(), fieldDefinition.NodeGroup);
                        break;
                }
            }

            result = "Success.";

            return fieldDefinition;
        }
        catch (Exception e)// A variety of exceptions can take place if the file is invalid, but we will always want to return null.
        {
            result = e.Message;

            // If the file is invalid, return null.
            return null;
        }
        finally
        {
            // Closes the reader.
            reader.Close();
        }
    }

    /// <summary>
    /// Reads the subtree of a PropertySet element.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="fieldDefinition"></param>
    private static void ReadPropertySet_2_3(XmlReader reader, FieldDefinition fieldDefinition)
    {
        // Creates a new PropertySet.
        PropertySet propertySet = new PropertySet();

        foreach (string name in IOUtilities.AllElements(reader))
        {
            switch (name)
            {
                case "PropertySet":
                    // Assigns the ID attribute value to the PropertySetID property.
                    propertySet.PropertySetID = reader["ID"];
                    break;
                case "BoxCollider":
                    // Assigns the BoxCollider read by the XmlReader to the PropertySet's Collider property.
                    propertySet.Collider = ReadBoxCollider_2_2(reader.ReadSubtree());
                    break;
                case "SphereCollider":
                    // Assigns the SphereCollider read by the XmlReader to the PropertySet's Collider property.
                    propertySet.Collider = ReadSphereCollider_2_2(reader.ReadSubtree());
                    break;
                case "MeshCollider":
                    // Assigns the MeshCollider read by the XmlReader to the PropertySet's Collider property.
                    propertySet.Collider = ReadMeshCollider_2_2(reader.ReadSubtree());
                    break;
                case "Separated":
                    // Assings the Separated attribute value to the Separated property.
                    propertySet.Separated = reader.ReadElementContentAsBoolean();
                    break;
                case "Friction":
                    // Assings the Friction attribute value to the Friction property.
                    propertySet.Friction = reader.ReadElementContentAsInt();
                    break;
                case "Mass":
                    // Assings the Mass attribute value to the Mass property.
                    propertySet.Mass = float.Parse(reader.ReadElementContentAsString());
                    break;
                case "Joint":
                    // Assings the Mass attribute value to the Mass property.
                    propertySet.Joint = ReadFieldJoint_2_3(reader.ReadSubtree());
                    break;
            }
        }

        // Adds the PropertySet to the fieldDefinition.
        fieldDefinition.AddPropertySet(propertySet);
    }

    /// <summary>
    /// Reads a field joint with the given XmlReader and returns the reading.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static PropertySet.FieldJoint ReadFieldJoint_2_3(XmlReader reader)
    {
        PropertySet.FieldJoint joint = new PropertySet.FieldJoint(new BXDVector3(), new BXDVector3());

        foreach (string name in IOUtilities.AllElements(reader))
        {
            switch (name)
            {
                case "BXDVector3":
                    switch (reader["VectorID"])
                    {
                        case "Center":
                            // Assign the BXDVector3 to the center.
                            joint = new PropertySet.FieldJoint(ReadBXDVector3_2_2(reader.ReadSubtree()), joint.Axis);
                            break;
                        case "Axis":
                            // Assign the BXDVector3 to the axis.
                            joint = new PropertySet.FieldJoint(joint.Center, ReadBXDVector3_2_2(reader.ReadSubtree()));
                            break;
                    }
                    break;
            }
        }

        return joint;
    }
}
